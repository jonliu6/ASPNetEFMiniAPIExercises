Test with PostMan since Swagger may need additional configuration for form data, specify request method, request body with form-data, request key-value pairs:
GET: https://localhost:7081/actors/4
Sample JSON response
  {
    "id": 4,
    "name": "Delta",
    "dateOfBirth": "2015-01-03T00:00:00",
    "picture": "https://localhost:7081/actors/8209bf7c-e60a-4078-a474-96363da3e0ab.png"
  }

POST: https://localhost:7081/actors
  Text: name, dateofbirth
  File: picture
Sample JSON response
  {
    "id": 10,
    "name": "Delta",
    "dateOfBirth": "2005-01-02T00:00:00",
    "picture": "https://localhost:7081/actors/0967634b-642a-4b26-ad3d-9001684b0807.jpg"
  }

Steps to implement:
1. create Entity classes
2. create repository interfaces and classes
3. add and update connection string in appsettings.json
4. create and modify application DB context
5. create DTO classes and auto-mapper files
6. create and modify EndPoint static classes
7. modify Program.cs to add services, mapGroup etc

For authentication JWT Token, use the following .Net command
dotnet user-jwts create

https://generate.plus/en/base64 to generate a secret

https://jwt.io to see the Token
