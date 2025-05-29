-- Associer des rôles aux utilisateurs

DECLARE @userId3 nvarchar(450) = (SELECT Id FROM AspNetUsers WHERE Email = 'admin1@gmail.com')
DECLARE @roleId3 nvarchar(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Admin')
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES (@userId3, @roleId3)



