Create Database Symphony_Ltd;
use Symphony_Ltd;
--UsersTbl table
CREATE TABLE UsersTbl (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) NOT NULL CHECK (Role IN ('Admin', 'User')),
    CreatedAt DATETIME DEFAULT GETDATE()
	);
INSERT INTO UsersTbl (Username, Email, Password, Role) 
VALUES ('Faham', 'mfaham871@gmail.com', 'AQAAAAEAACcQAAAAEAJzNujGnQW1QC4w//RR+XnD/xVPydNE6jqLzR9VflrjOya6puHdkjyScr5PVTmttg==', 'Admin');
--Instructors table
CREATE TABLE Instructors (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NULL,
    ImagePath NVARCHAR(255) NULL
);
--Courses table
CREATE TABLE Courses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL, -- Or TEXT for larger descriptions
    VideoUrl NVARCHAR(255) NULL, -- Or store file path if you upload directly
    Type NVARCHAR(50) NOT NULL CHECK (Type IN ('Free', 'Premium')),
    CreatedAt DATETIME DEFAULT GETDATE()
);
--PremiumAccess table
CREATE TABLE PremiumAccess (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    TransactionId NVARCHAR(255) NOT NULL, -- Payment transaction ID
    Amount DECIMAL(10, 2) NOT NULL, -- Payment amount
    Status NVARCHAR(50) NOT NULL CHECK (Status IN ('Pending', 'Approved', 'Rejected')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES UsersTbl(Id)
);
--ContactMessages table
CREATE TABLE ContactMessages (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Subject NVARCHAR(255) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsRead BIT DEFAULT 0 -- To track if admin has read the message
);
-- Exam Table
CREATE TABLE Exams (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CourseId INT NOT NULL,
    ExamDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CourseId) REFERENCES Courses(Id)
);
-- Questions Table
CREATE TABLE Questions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ExamId INT NOT NULL,
    QuestionText NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (ExamId) REFERENCES Exams(Id)
);
-- Options Table
CREATE TABLE Options (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    QuestionId INT NOT NULL,
    OptionText NVARCHAR(255) NOT NULL,
    IsCorrect BIT NOT NULL DEFAULT 0, -- 1 for correct, 0 for incorrect
    FOREIGN KEY (QuestionId) REFERENCES Questions(Id)
);
-- ExamResults Table
CREATE TABLE ExamResults (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ExamId INT NOT NULL,
    UserId INT NOT NULL,
    Score INT NOT NULL,
    ResultDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ExamId) REFERENCES Exams(Id),
    FOREIGN KEY (UserId) REFERENCES UsersTbl(Id)
);
-- Books Table
CREATE TABLE Books (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Author NVARCHAR(255) NOT NULL,
    ISBN NVARCHAR(50) UNIQUE NOT NULL,
    PublishedDate DATE NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
--UsersTbl select
truncate table UsersTbl;
select * from UsersTbl;
--Instructors select
truncate table Instructors;
select * from Instructors;
--Courses select
truncate table Courses;
select * from Courses;
--PremiumAccess select
truncate table PremiumAccess;
select * from PremiumAccess;
--ContactMessages select
truncate table ContactMessages;
select * from ContactMessages;
--Exams select
truncate table Exams;
select * from Exams;
--Questions select
truncate table Questions;
select * from Questions;
--Options select
truncate table Options;
select * from Options;
--ExamResults select
truncate table ExamResults;
select * from ExamResults;
--Books select
truncate table Books;
select * from Books;



