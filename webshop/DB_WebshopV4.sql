
/* webshop V4 */ 

use webshopResetPassword;

ALTER TABLE Customer
ADD Token nvarchar(100);



/* Manufacturer */ 
CREATE TABLE Manufacturer(
Manufacturer_ID int IDENTITY not null, 
Manufacturer_Name nvarchar(60),
FirstName nvarchar(30),
LastName nvarchar(30),
INDEX index_Maincategory_Name(Manufacturer_Name),
Constraint PK_MC_MCID Primary Key (Manufacturer_ID));

/* Customer */ 
CREATE TABLE Customer(
Customer_ID int IDENTITY not null, 
Title nvarchar(30),
FirstName nvarchar(30),
LastName nvarchar(30),
Email nvarchar(50),
Street nvarchar(60),
Zip int not null,
City nvarchar(50),
PwHash binary(32),
Salt binary(32),
Constraint PK_U_UID Primary Key (Customer_ID));


/* Category */ 
CREATE TABLE Category(
Category_ID int IDENTITY not null,
Category_Name nvarchar(30),
TaxRate float not null,
Constraint PK_SC_SCID Primary Key (Category_ID));




/* Product */ 
CREATE TABLE Product(
Product_ID int IDENTITY not null, 
Manufacturer_ID int not null, 
Category_ID int not null, 
Product_Name nvarchar(100),
NetUnitPrice money not null,
ImagePath nvarchar(50),
Description nvarchar(50),
Constraint PK_P_PID Primary Key (Product_ID),
CONSTRAINT FK_P_SCID FOREIGN KEY (Manufacturer_ID) REFERENCES Manufacturer (Manufacturer_ID),
CONSTRAINT FK_P_CAID FOREIGN KEY (Category_ID) REFERENCES Category (Category_ID));


/* Order*/ 
CREATE TABLE OrderTable(
Order_ID int IDENTITY not null, 
Customer_ID int not null, 
PriceTotal money not null,
DateOrdered Date not null,
Street nvarchar(30),
Zip int not null,
City nvarchar(30),
FirstName nvarchar(30),
LastName nvarchar(30),
Constraint PK_OP_OPID Primary Key (Order_iD),
CONSTRAINT FK_P_PID FOREIGN KEY (Customer_ID) REFERENCES Customer (Customer_ID));


/* OrderLine */ 
CREATE TABLE OrderLine(
OrderLine_ID int IDENTITY not null, 
Order_ID int not null, 
Product_ID int not null, 
Amount int not null,
NetUnitPrice money not null,
TaxRate float not null,
Constraint PK_P_OLID Primary Key (OrderLine_ID),
CONSTRAINT FK_P_OOID FOREIGN KEY (Order_ID) REFERENCES OrderTable (Order_ID),
CONSTRAINT FK_P_PRID FOREIGN KEY (Product_ID) REFERENCES Product (Product_ID));




/* New */ 

/* Manufacturer */ 
SET IDENTITY_INSERT Manufacturer ON;

INSERT INTO  Manufacturer (Manufacturer_ID,Manufacturer_Name,FirstName,LastName)
Values('1','Samsung','Albert','Kranz'),
('2','Corsair','Erich','Bauer'),
('3','Toshiba','Sibylle','Schneider'),
('4','Apple','Arnold','Hase'),
('5','HP','Jana','Reichelt'),
('6','MSI','Herbert','Huber'),
('7','Microsoft','Albert','Schhweizer'),
('8','Sony','Yoshy','Lazer'),
('9','Nintendo','Hashimoto','Honda'),
('10','Razer','Jonathan','Fraser');

SET IDENTITY_INSERT Manufacturer Off;

/* Category */ 
SET IDENTITY_INSERT Category ON;

INSERT INTO  Category(Category_ID,Category_Name,TaxRate)
Values('1','Software','20'),
('2','Hardware','20'),
('3','PC','20'),
('4','Notebook','20'),
('5','Smartphone/Tablet','20'),
('6','Outlet','20'),
('7','Assecoires','20'),
('8','Consoles','20'),
('9','Videogames','20');

SET IDENTITY_INSERT Category Off;

/* Products */ 

/* Samsung */ 

INSERT INTO  Product(Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice, ImagePath)
Values('1','2','256 GB Samsung 870 EVO SSD','49.99','~/Images/Samsung870EVO.jpg');

INSERT INTO  Product(Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice, ImagePath)
Values('9','4','Razer Gaming Notebook Kraken 2000','1899.99','~/Images/RazerNotebook.jpg');

INSERT INTO  Product(Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice, ImagePath)
Values('4','5','Apple Ipad Pro 2020 WIFI 256 GB +','999.99','~/Images/AppleIpad.jpg');

INSERT INTO  Product(Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice, ImagePath)
Values
('1','2','512 GB Samsung 870 EVO SSD','79.99','~/Images/Samsung870EVO.jpg'),
('1','2','1 TB Samsung 870 EVO SSD','99.99','~/Images/Samsung870EVO.jpg'),
('1','2','2 TB Samsung 870 EVO SSD','149.99','~/Images/Samsung870EVO.jpg'),
('1','2','256 GB Samsung 980 NVMe m2 SSD','59.99','~/Images/samsung980M2.jpg'),
('1','2','512 GB Samsung 980 NVMe m2 SSD','89.99','~/Images/samsung980M2.jpg'),
('1','2','1 TB Samsung 980 NVMe m2 SSD','109.99','~/Images/samsung980M2.jpg'),
('1','2','2 TB Samsung 980 NVMe m2 SSD','159.99','~/Images/samsung980M2.jpg'),
('1','2','256 GB Samsung 980 NVMe m2 PRO SSD','79.99','~/Images/samsung980M2PRO.jpg'),
('1','2','512 GB Samsung 980 NVMe m2 PRO SSD','109.99','~/Images/samsung980M2PRO.jpg'),
('1','2','1 TB Samsung 980 NVMe m2 PRO SSD','129.99','~/Images/samsung980M2PRO.jpg'),
('1','2','2 TB Samsung 980 NVMe m2 PRO SSD','199.99','~/Images/samsung980M2PRO.jpg');

/* Corsair */ 

INSERT INTO  Product(Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice, ImagePath)
Values('2','2','256 GB Corsair MP510 SSD','49.99','~/Images/corsairMP510.jpg'),
('2','2','512 GB Corsair MP510O SSD','79.99','~/Images/corsairMP510.jpg'),
('2','2','1 TB Corsair MP510 EVO SSD','99.99','~/Images/corsairMP510.jpg'),
('2','2','2 TB Corsair MP510 EVO SSD','149.99','~/Images/corsairMP510.jpg'),
('2','2','256 GB Corsair MP600 NVMe m2 SSD','59.99','~/Images/corsairMP600.jpg'),
('2','2','512 GB Corsair MP600NVMe m2 SSD','89.99','~/Images/corsairMP600.jpg'),
('2','2','1 TB Corsair MP600 NVMe m2 SSD','109.99','~/Images/corsairMP600.jpg'),
('2','2','2 TB Corsair MP600 NVMe m2 SSD','159.99','~/Images/corsairMP600.jpg'),
('2','2','256 GB Corsair MP600 Pro NVMe m2 PRO SSD','79.99','~/Images/corsairMP600.jpg'),
('2','2','512 GB Corsair MP600 Pro  NVMe m2 PRO SSD','109.99','~/Images/corsairMP600.jpg'),
('2','2','1 TB Corsair MP600 Pro  NVMe m2 PRO SSD','129.99','~/Images/corsairMP600.jpg'),
('2','2','2 TB Corsair MP600 Pro  NVMe m2 PRO SSD','199.99','~/Images/corsairMP600.jpg');

/* Toshiba */ 

INSERT INTO  Product(Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice, ImagePath)
Values('3','4','Toshiba N4256 Notebook','499.99','~/Images/ToshibaLapttop.jpg'),
('3','4','Toshiba N4256 Notebook 2','999.99','~/Images/ToshibaLapttop.jpg'),
('3','4','Toshiba N4255 Notebook 2 Pro','729.99','~/Images/ToshibaLapttop.jpg'),
('3','4','Toshiba N4225 Notebook z56','1099.99','~/Images/ToshibaLapttop.jpg'),
('3','2','256 GB Toshiba X85Z32 HDD','29.99','~/Images/ToshibaHDD.jpg'),
('3','2','512 GB Toshiba X85Z32 HDD','79.99','~/Images/ToshibaHDD.jpg'),
('3','2','1 TB Toshiba X85Z32 HDD','99.99','~/Images/ToshibaHDD.jpg'),
('3','2','2 TB Toshiba X85Z32 HDD','149.99','~/Images/ToshibaHDD.jpg'),
('3','2','1 TB Toshiba X9453 Super HDD','109.99','~/Images/ToshibaHDD.jpg'),
('3','2','2 TB Toshiba X9453 Super HDD','159.99','~/Images/ToshibaHDD.jpg'),
('3','2','4 TB Toshiba X9453 Super HDD','129.99','~/Images/ToshibaHDD.jpg'),
('3','2','8 TB Toshiba X9453 Super HDD','199.99','~/Images/ToshibaHDD.jpg');

/* Apple */ 



INSERT INTO  Product(Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice, ImagePath)
Values('4','4','Apple Macbook 2019 ','1299.99','~/Images/ToshibaLapttop.jpg'),
('4','4','Apple Macbook 2020 ','1499.99','~/Images/ToshibaLapttop.jpg'),
('4','4','Apple Macbook 2021 ','2499.99','~/Images/ToshibaLapttop.jpg'),
('4','4','Apple Macbook 2021 Pro ','2999.99','~/Images/ToshibaLapttop.jpg'),
('4','4','Apple Macbook 2021 Pro Collectors Edition ','3499.99','~/Images/ToshibaLapttop.jpg'),
('4','5','Apple Iphone 8','129.99','~/Images/AppleIphone.jpg'),
('4','5','Apple Iphone 9','299.99','~/Images/AppleIphone.jpg'),
('4','5','Apple Iphone 10','499.99','~/Images/AppleIphone.jpg'),
('4','5','Apple Iphone 11','849.99','~/Images/AppleIphone.jpg'),
('4','5','Apple Iphone 11 +','999.99','~/Images/AppleIphone.jpg'),
('4','5','Apple Ipad 8 +','399.99','~/Images/AppleIpad.jpg'),
('4','5','Apple Ipad Air 4 +','599.99','~/Images/AppleIpad.jpg'),
('4','5','Apple Ipad Pro 2020 WIFI 256 GB +','999.99','~/Images/AppleIpad.jpg'),
('4','5','Apple Ipad Pro 2020 WIFI 512 GB +','1099.99','~/Images/AppleIpad.jpg'),
('4','5','Apple Ipad Pro 2020 WIFI + Cellular 1 TB +','1299.99','~/Images/AppleIpad.jpg');

/* HP */ 

INSERT INTO  Product(Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice, ImagePath)
Values('5','3',' HP PC-Build Z2-G2 ','1299.99','~/Images/HPZ2.jpg'),
('5','3',' HP PC-Build Z2-G2 ','1299.99','~/Images/HPZ2.jpg'),
('5','3',' HP PC-Build Z2-G2 ','1299.99','~/Images/HPZ2.jpg'),
('5','3',' HP PC-Build Z4-G2 ','2299.99','~/Images/HPZ4.jpg'),
('5','3',' HP PC-Build Z4-G3 ','2599.99','~/Images/HPZ4.jpg'),
('5','3',' HP PC-Build Z4-G4 ','2799.99','~/Images/HPZ4.jpg'),
('5','3',' HP PC-Build Z6-G1 ','3299.99','~/Images/HPZ6.jpg'),
('5','3',' HP PC-Build Z6-G2 ','4299.99','~/Images/HPZ6.jpg'),
('5','3',' HP PC-Build Z6-G4 ','5299.99','~/Images/HPZ6.jpg');

/* MSi */ 

INSERT INTO  Product(Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice, ImagePath)
Values('6','4','MSI Gaming Notebook XP222','1499.99','~/Images/MSINoteBook.jpg'),
('6','4','MSI Gaming Notebook XP333','1799.99','~/Images/MSINoteBook.jpg'),
('6','4','MSI Gaming Notebook XP555 ','2299.99','~/Images/MSINoteBook.jpg'),
('6','4','MSI Gaming Notebook XP Pro ','2499.99','~/Images/MSINoteBook.jpg');


/* Microsoft */ 

INSERT INTO  Product(Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice, ImagePath)
Values('7','1','Windows 7 Home','10.99','~/Images/win7.jpg'),
('7','1','Windows 7 Professional','14.99','~/Images/win7.jpg'),
('7','1','Windows 7 Enterprise','149.99','~/Images/win7.jpg'),
('7','1','Windows 10 Home','14.99','~/Images/Win10.jpg'),
('7','1','Windows 10 Proffessional','19.99','~/Images/Win10.jpg'),
('7','1','Windows 10 Enterprise','249.99','~/Images/Win10.jpg'),
('7','4','Windows Surface laptop 2020','1719.99','~/Images/MicrosoftSurfaceLaptop.jpg'),
('7','4','Windows Surface laptop 2020 Pro','2119.99','~/Images/MicrosoftSurfaceLaptop.jpg'),
('7','4','Windows Surface laptop 2021','2499.99','~/Images/MicrosoftSurfaceLaptop.jpg'),
('7','4','Windows Surface laptop 2021 Pro','2999.99','~/Images/MicrosoftSurfaceLaptop.jpg');

/* Sony */ 

INSERT INTO  Product(Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice, ImagePath)
Values('8','8','Sony Playstation 4 256 GB','229.99','~/Images/playstation4.jpg'),
('8','8','Sony Playstation 4 512 GB','279.99','~/Images/playstation4.jpg'),
('8','8','Sony Playstation 4 1 TB GB','299.99','~/Images/playstation4.jpg'),
('8','8','Sony Playstation 4 Pro 256 GB','399.99','~/Images/playstation4.jpg'),
('8','8','Sony Playstation 4 Pro 512 GB','429.99','~/Images/playstation4.jpg'),
('8','8','Sony Playstation 4 Pro TB GB','449.99','~/Images/playstation4.jpg'),
('8','6','Sony Playstation 4 256 GB Refurbished','179.99','~/Images/playstation4.jpg'),
('8','6','Sony Playstation 4 512 GB Refurbished','199.99','~/Images/playstation4.jpg'),
('8','6','Sony Playstation 4 1 TB GB Refurbished','220.99','~/Images/playstation4.jpg'),
('8','6','Sony Playstation 4 Pro 256 GB Refurbished','349.99','~/Images/playstation4.jpg'),
('8','6','Sony Playstation 4 Pro 512 GB Refurbished','379.99','~/Images/playstation4.jpg'),
('8','6','Sony Playstation 4 Pro TB GB Refurbished','399.99','~/Images/playstation4.jpg'),
('8','7','Sony Playstation 4 Controller Black','39.99','~/Images/ps4controllerblack.jpg'),
('8','7','Sony Playstation 4 Controller Black Dual Pack','69.99','~/Images/ps4controllerblack.jpg'),
('8','7','Sony Playstation 4 Controller Red','39.99','~/Images/ps4controllerred.jpg'),
('8','7','Sony Playstation 4 Controller Red Dual Pack','69.99','~/Images/ps4controllerred.jpg'),
('8','7','Sony Playstation 4 Controller White','39.99','~/Images/ps4controllerwhite.jpg'),
('8','7','Sony Playstation 4 Controller White Dual Pack','69.99','~/Images/ps4controllerwhite.jpg'),
('8','9','Sony Playstation 4 Ghost of Tsushima Vido Game','59.99','~/Images/ps4ghost.jpg'),
('8','9','Sony Playstation 4 Ghost of Tsushima Vido Game Digital','49.99','~/Images/ps4ghost.jpg'),
('8','9','Sony Playstation 4 Crash Bandicoot 4 It is About Time Vido Game Digital Version','59.99','~/Images/ps4crash.jpg'),
('8','9','Sony Playstation 4 Crash Bandicoot 4 It is About Time Vido Game Hardcase Version','59.99','~/Images/ps4crash.jpg');


/* Nintendo */ 

INSERT INTO  Product(Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice, ImagePath)
Values('9','8','Nintendo Switch Standard version','299.99','~/Images/NintendoSwtich.jpg'),
('9','8','Nintendo Switch Deluxe Edition','399.99','~/Images/NintendoSwtich.jpg'),
('9','6','Nintendo Switch Standard version Refurbished','199.99','~/Images/NintendoSwtich.jpg'),
('9','6','Nintendo Switch Deluxe Edition Refurbished','299.99','~/Images/NintendoSwtich.jpg'),
('9','6','Nintendo Super Mario 3D World digital Version','49.99','~/Images/nintendoMario.jpg'),
('9','6','Nintendo Super Mario 3D World Hardcase Version','69.99','~/Images/nintendoMario.jpg'),
('9','6','Nintendo Controller','59.99','~/Images/nintendoSwitchController.jpg'),
('9','6','Nintendo Controller Dual Pack','89.99','~/Images/nintendoSwitchController.jpg');


/* Razer */ 

INSERT INTO  Product(Manufacturer_ID,Category_ID,Product_Name,NetUnitPrice, ImagePath)
Values
('10','4','Razer Gaming Notebook Kraken 3000','2299.99','~/Images/RazerNotebook.jpg'),
('10','4','Razer Gaming Notebook Kraken 4000','3299.99','~/Images/RazerNotebook.jpg'),
('10','4','Razer Gaming Mouse','59.99','~/Images/RazerNotebook.jpg'),
('10','4','Razer Gaming Notebook Kraken 4000','3299.99','~/Images/RazerNotebook.jpg');
