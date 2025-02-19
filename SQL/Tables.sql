﻿CREATE TABLE Genre (
  id INT IDENTITY PRIMARY KEY,
  name NVARCHAR(100) NOT NULL,
);

CREATE TABLE Actor (
  id INT IDENTITY PRIMARY KEY,
  name NVARCHAR(100) NOT NULL,
  dateofbirth DATETIME,
  picture NVARCHAR(300)
);

CREATE TABLE Movie (
  id INT IDENTITY PRIMARY KEY,
  title NVARCHAR(200) NOT NULL,
  isReleased BIT,
  releaseDate DATETIME,
  poster NVARCHAR(300)
);

CREATE TABLE Comment (
  id INT IDENTITY PRIMARY KEY,
  body NVARCHAR(100) NOT NULL,
  movieId INT
);

CREATE TABLE ActorMovie (
  actorId INT,
  movieId INT,
  [order] INT,
  character NVARCHAR(50),
  CONSTRAINT pk_actor_movie PRIMARY KEY (actorId, movieId)
);