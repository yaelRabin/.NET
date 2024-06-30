create table Appointment
(
	AppointmentId int primary key identity(200,1),
	ArrivalTime DateTime not null,
	UserId int not null foreign key references [User](UserId),
	RequestTime DateTime not null
)