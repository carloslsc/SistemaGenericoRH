CREATE DATABASE db_sistema_generico_rh;

USE [db_sistema_generico_rh]

CREATE SCHEMA seguridad;

CREATE TABLE seguridad.usuarios(
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[usuario] [varchar](50) NOT NULL,
	[contrasena] [varchar](250) NOT NULL,
	[correo] [varchar] (50) NOT NULL,
	[sexo] [char] (1) NOT NULL,
	[estatus] [bit] NOT NULL,
	[fecha_alta] [datetime] NOT NULL,
	[usuario_alta] [bigint] NOT NULL,
	[fecha_modificacion] [datetime] NULL,
	[usuario_modificacion] [bigint] NULL
 CONSTRAINT [PK_usuarios_user] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE seguridad.usuarios ADD CONSTRAINT UQ__Constrai_User UNIQUE (usuario)
GO

ALTER TABLE seguridad.usuarios ADD CONSTRAINT UQ__Constrai_Correo UNIQUE (correo)
GO

INSERT INTO seguridad.usuarios (usuario, contrasena, correo, sexo, estatus, fecha_alta, usuario_alta, fecha_modificacion, 
usuario_modificacion) values ('usuario123', '4ff842f562d87f0758493a0d1807b2ff0a814ffe29f31bed5fe8aad2050b5f90', 'correo@correo.com', 'M', 1, GETDATE(), 1, GETDATE(), NULL);

----Contraseña es Aaz1234567