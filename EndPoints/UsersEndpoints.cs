using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIsWithASPNetEF.DTOs;
using MinimalAPIsWithASPNetEF.Filters;
using MinimalAPIsWithASPNetEF.Services;
using MinimalAPIsWithASPNetEF.Utilities;
using MinimalAPIsWithASPNetEF.Validations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MinimalAPIsWithASPNetEF.EndPoints
{
    public static class UsersEndpoints
    {
        public static RouteGroupBuilder MapUsers( this RouteGroupBuilder group)
        {
            group.MapPost("/register", Register)
                .AddEndpointFilter<GenericValidationFilter<UserCredentialsDTO>>();
            group.MapPost("/login", Login) // used to get valid token
                .AddEndpointFilter<GenericValidationFilter<UserCredentialsDTO>>();
            group.MapPost("/makeadmin", MakeAdmin).AddEndpointFilter<GenericValidationFilter<EditClaimDTO>>()
                .RequireAuthorization("isadmin");
            group.MapDelete("/removeadmin", RemoveAdmin).AddEndpointFilter<GenericValidationFilter<EditClaimDTO>>()
                .RequireAuthorization("isadmin");
            group.MapGet("/renewtoken", RenewToken).RequireAuthorization(); // renew token can be done by any user

            return group;
        }

        static async Task<Results<Ok<AuthenticationResponseDTO>, BadRequest<IEnumerable<IdentityError>>>> Register(UserCredentialsDTO userCredentialDto,
            [FromServices] UserManager<IdentityUser> userManager, IConfiguration conf)
        {
            var user = new IdentityUser
            {
                UserName = userCredentialDto.Email,
                Email = userCredentialDto.Email
            };

            var result = await userManager.CreateAsync(user, userCredentialDto.Password);
            if (result.Succeeded)
            {
                var authenticationResp = await BuildToken(userCredentialDto, conf, userManager);
                return TypedResults.Ok(authenticationResp);

            }
            else
            {
                return TypedResults.BadRequest(result.Errors);
            }
        }

        async static Task<Results<Ok<AuthenticationResponseDTO>, BadRequest<string>>> Login(UserCredentialsDTO userCredentialDto,
            [FromServices] SignInManager<IdentityUser> signInMgr,
            [FromServices] UserManager<IdentityUser> usrMgr,
            IConfiguration cfg)
        {
            var usr = await usrMgr.FindByEmailAsync(userCredentialDto.Email);
            if (usr is null)
            {
                return TypedResults.BadRequest("There was a problem with the email or the password.");
            }

            var results = await signInMgr.CheckPasswordSignInAsync(usr, userCredentialDto.Password, lockoutOnFailure: false);
            if (results.Succeeded)
            {
                var authResp = await BuildToken(userCredentialDto, cfg, usrMgr);
                return TypedResults.Ok(authResp);
            }
            else
            {
                return TypedResults.BadRequest("There was a problem with the email or the password.");
            }
        }

        async static Task<Results<NoContent, NotFound>> MakeAdmin([FromBody] EditClaimDTO emailClaimDto, 
            [FromServices] UserManager<IdentityUser> usrMgr)
        {
            var usr = await usrMgr.FindByEmailAsync(emailClaimDto.Email);

            if (usr is null) {
                return TypedResults.NotFound();
            }

            await usrMgr.AddClaimAsync(usr, new Claim("isadmin", "true"));
            return TypedResults.NoContent();
            // for the 1st isadmin user, go to the  table and insert a record with UserId, "isadmin", true
        }

        async static Task<Results<NoContent, NotFound>> RemoveAdmin([FromBody] EditClaimDTO emailClaimDto,
            [FromServices] UserManager<IdentityUser> usrMgr)
        {
            var usr = await usrMgr.FindByEmailAsync(emailClaimDto.Email);

            if (usr is null)
            {
                return TypedResults.NotFound();
            }

            await usrMgr.RemoveClaimAsync(usr, new Claim("isadmin", "true"));
            return TypedResults.NoContent();
        }

        private async static Task<Results<NotFound, Ok<AuthenticationResponseDTO>>> RenewToken( IUsersService usrSvc,
            IConfiguration cfg,
            [FromServices] UserManager<IdentityUser> usrMgr)
        {
            var usr = await usrSvc.GetUser();

            if (usr is null)
            {
                return TypedResults.NotFound();
            }

            var usrCredential = new UserCredentialsDTO { Email = usr.Email!};
            var resp = await BuildToken(usrCredential, cfg, usrMgr);

            return TypedResults.Ok(resp);
        } 

        private async static Task<AuthenticationResponseDTO> BuildToken(UserCredentialsDTO userCredentialsDto, IConfiguration cfg, UserManager<IdentityUser> userManager)
        {
            var claims = new List<Claim>
            {
                new Claim("email", userCredentialsDto.Email),
                new Claim("Whatever I want", "this is the value")

            };

            var user = await userManager.FindByNameAsync(userCredentialsDto.Email);
            var claimFromDB = await userManager.GetClaimsAsync(user!);

            claims.AddRange(claimFromDB);

            var key = KeysHandler.GetKey(cfg).First();
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);  // expiration 1 year from now on

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new AuthenticationResponseDTO
            {
                Token = token,
                Expiration = expiration
            };
        }
    }
}
