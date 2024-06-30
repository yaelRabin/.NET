 create table [User]
(
	UserId int primary key identity(100,10),
	UserName varchar(35) unique not null,
	[Name] varchar(35) not null ,
	UserPassword varchar(250) not null,
	Phone varchar(15) unique not null
)