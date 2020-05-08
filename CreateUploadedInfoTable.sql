Create Table UploadedInfo(
Id BigInt Identity(1,1) Primary Key,
TransactionId Varchar(100) Not Null,
Amount Varchar(100) Not Null,
CurrencyCode Varchar(100) Not Null,
TransactionDate DateTime,
StatusCode Varchar(100) Not Null)