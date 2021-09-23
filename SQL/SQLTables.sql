USE JobApplicationDb;

CREATE TABLE personal_info (
	[User ID] int NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[First Name] NVARCHAR(20) NOT NULL,
	[Last Name] NVARCHAR(20) NOT NULL,
	[Birth Date] DATE NOT NULL,
	);
	CREATE TABLE experiences(
	[User ID] int NOT NULL REFERENCES personal_info([User ID]),
	[Company Name] nvarchar(MAX) NOT NULL,
	[Start Year] int NOT NULL,
	[End Year] int NOT NULL,
	);

CREATE TABLE expectations(
	[User ID] int NOT NULL REFERENCES personal_info([User ID]),
	Salary int NOT NULL,
	Additional nvarchar(MAX)
	);


ALTER TABLE personal_infor
ADD CONSTRAINT personal_infor Primary Key ([User ID]);

SELECT * from personal_info;
SELECT * from experiences;
SELECT * from expectations;

SELECT p.*, exp.[Company Name], exp.[Start Year], exp.[End Year], e.Salary, e.Additional from personal_info p 
INNER JOIN experiences exp on p.[User ID] = exp.[User ID] 
INNER JOIN expectations e on p.[User ID] = e.[User ID];
