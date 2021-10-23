create database quotation;

use quotation;

CREATE TABLE `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

CREATE TABLE `AspNetRoles` (
    `Id` nvarchar(450) NOT NULL,
    `Name` nvarchar(256) NULL,
    `NormalizedName` nvarchar(256) NULL,
    `ConcurrencyStamp` longtext NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `AspNetUsers` (
    `Id` nvarchar(450) NOT NULL,
    `UserName` nvarchar(256) NULL,
    `NormalizedUserName` nvarchar(256) NULL,
    `Email` nvarchar(256) NULL,
    `NormalizedEmail` nvarchar(256) NULL,
    `EmailConfirmed` tinyint NOT NULL,
    `PasswordHash` longtext NULL,
    `SecurityStamp` longtext NULL,
    `ConcurrencyStamp` longtext NULL,
    `PhoneNumber` longtext NULL,
    `PhoneNumberConfirmed` tinyint NOT NULL,
    `TwoFactorEnabled` tinyint NOT NULL,
    `LockoutEnd` datetime(6) NULL,
    `LockoutEnabled` tinyint NOT NULL,
    `AccessFailedCount` int NOT NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `AspNetRoleClaims` (
    `Id` int NOT NULL,
    `RoleId` nvarchar(450) NOT NULL,
    `ClaimType` longtext NULL,
    `ClaimValue` longtext NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserClaims` (
    `Id` int NOT NULL,
    `UserId` nvarchar(450) NOT NULL,
    `ClaimType` longtext NULL,
    `ClaimValue` longtext NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserLogins` (
    `LoginProvider` nvarchar(128) NOT NULL,
    `ProviderKey` nvarchar(128) NOT NULL,
    `ProviderDisplayName` longtext NULL,
    `UserId` nvarchar(450) NOT NULL,
    PRIMARY KEY (`LoginProvider`, `ProviderKey`),
    CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserRoles` (
    `UserId` nvarchar(450) NOT NULL,
    `RoleId` nvarchar(450) NOT NULL,
    PRIMARY KEY (`UserId`, `RoleId`),
    CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `AspNetUserTokens` (
    `UserId` nvarchar(450) NOT NULL,
    `LoginProvider` nvarchar(128) NOT NULL,
    `Name` nvarchar(128) NOT NULL,
    `Value` longtext NULL,
    PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
    CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);

CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);

CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);

CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);

CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);

CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);

CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('00000000000000_CreateIdentitySchema', '3.1.6');

ALTER TABLE `AspNetUsers` ADD `Name` nvarchar(200) NOT NULL DEFAULT '';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200716070842_QuotationUserAdded', '3.1.6');

ALTER TABLE `AspNetUsers` ADD `CustomerCatagory` int NOT NULL DEFAULT 0;

CREATE TABLE `Products` (
    `ProductId` nvarchar(450) NOT NULL,
    `ProductName` longtext NOT NULL,
    `ProductFullSheetPrice` real NOT NULL,
    `ProductPieceCutPrice` real NOT NULL,
    PRIMARY KEY (`ProductId`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200716120005_Products_Introduced', '3.1.6');

ALTER TABLE `Products` ADD `CreatedByUserId` nvarchar(450) NOT NULL DEFAULT '';

ALTER TABLE `Products` ADD `ModifiedByUserId` nvarchar(450) NULL;

ALTER TABLE `Products` ADD `ProductCreationDate` datetime NOT NULL;

ALTER TABLE `Products` ADD `ProductModificationDate` datetime NOT NULL DEFAULT '0001-01-01 00:00:00.000000';

CREATE TABLE `QuotationNote` (
    `QuotationNoteId` nvarchar(450) NOT NULL,
    `Note` longtext NOT NULL,
    `IsNoteDefault` tinyint NOT NULL,
    PRIMARY KEY (`QuotationNoteId`)
);

CREATE TABLE `SpecialCharges` (
    `SpecialChargesId` nvarchar(450) NOT NULL,
    `SpecialChargeName` longtext NOT NULL,
    `SpecialChargeAmount` real NOT NULL,
    `IsDefault` tinyint NOT NULL,
    PRIMARY KEY (`SpecialChargesId`)
);

CREATE TABLE `Quotation` (
    `QuotationId` nvarchar(450) NOT NULL,
    `QuotationAmount` real NOT NULL,
    `QuotationTo` longtext NOT NULL,
    `QuotationCreationDate` datetime(6) NOT NULL,
    `QuotationModificationDate` datetime(6) NOT NULL,
    `CreatedByUserId` nvarchar(450) NOT NULL,
    `ModifiedByUserId` NVARCHAR(450) NULL,
    `QuotationNoteId` nvarchar(450) NOT NULL,
    PRIMARY KEY (`QuotationId`),
    CONSTRAINT `FK_Quotation_AspNetUsers_CreatedByUserId` FOREIGN KEY (`CreatedByUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Quotation_QuotationNote_QuotationNoteId` FOREIGN KEY (`QuotationNoteId`) REFERENCES `QuotationNote` (`QuotationNoteId`) ON DELETE RESTRICT
);

CREATE TABLE `QuotationDetails` (
    `QuotationDetailsId` nvarchar(450) NOT NULL,
    `SequenceNumber` int NOT NULL,
    `ProductName` longtext NOT NULL,
    `SheetMeasurementOptions` int NOT NULL,
    `SheetSizingOptions` int NOT NULL,
    `ProductDimensionX` real NOT NULL,
    `ProductDimensionY` real NOT NULL,
    `ProductQuantity` int NOT NULL,
    `ProductRate` real NOT NULL,
    `TotalArea` real NOT NULL,
    `ProductAmount` real NOT NULL,
    `QuotationId` nvarchar(450) NOT NULL,
    `ProductId` nvarchar(450) NOT NULL,
    PRIMARY KEY (`QuotationDetailsId`),
    CONSTRAINT `FK_QuotationDetails_Products_ProductId` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`ProductId`) ON DELETE RESTRICT,
    CONSTRAINT `FK_QuotationDetails_Quotation_QuotationId` FOREIGN KEY (`QuotationId`) REFERENCES `Quotation` (`QuotationId`) ON DELETE CASCADE
);

CREATE TABLE `QuotationSpecialCharges` (
    `SpecialChargesId` nvarchar(450) NOT NULL,
    `QuotationId` nvarchar(450) NOT NULL,
    PRIMARY KEY (`SpecialChargesId`, `QuotationId`),
    CONSTRAINT `FK_QuotationSpecialCharges_Quotation_QuotationId` FOREIGN KEY (`QuotationId`) REFERENCES `Quotation` (`QuotationId`) ON DELETE CASCADE,
    CONSTRAINT `FK_QuotationSpecialCharges_SpecialCharges_SpecialChargesId` FOREIGN KEY (`SpecialChargesId`) REFERENCES `SpecialCharges` (`SpecialChargesId`) ON DELETE CASCADE
);

CREATE INDEX `IX_Products_CreatedByUserId` ON `Products` (`CreatedByUserId`);

CREATE INDEX `IX_Quotation_CreatedByUserId` ON `Quotation` (`CreatedByUserId`);

CREATE INDEX `IX_Quotation_QuotationNoteId` ON `Quotation` (`QuotationNoteId`);

CREATE INDEX `IX_QuotationDetails_ProductId` ON `QuotationDetails` (`ProductId`);

CREATE INDEX `IX_QuotationDetails_QuotationId` ON `QuotationDetails` (`QuotationId`);

CREATE INDEX `IX_QuotationSpecialCharges_QuotationId` ON `QuotationSpecialCharges` (`QuotationId`);

ALTER TABLE `Products` ADD CONSTRAINT `FK_Products_AspNetUsers_CreatedByUserId` FOREIGN KEY (`CreatedByUserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200720060017_NewTablesAdded', '3.1.6');

ALTER TABLE AspNetUserTokens MODIFY `Name` nvarchar(128) NOT NULL;

ALTER TABLE AspNetUserTokens MODIFY `LoginProvider` nvarchar(128) NOT NULL;

ALTER TABLE AspNetUserLogins MODIFY `ProviderKey` nvarchar(450) NOT NULL;

ALTER TABLE AspNetUserLogins MODIFY `LoginProvider` nvarchar(450) NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200720063819_IdentityChanges', '3.1.6');

ALTER TABLE `SpecialCharges` ADD `CompanyId` nvarchar(450) NOT NULL DEFAULT '';

ALTER TABLE `QuotationSpecialCharges` ADD `CompanyId` nvarchar(450) NOT NULL DEFAULT '';

ALTER TABLE `QuotationNote` ADD `CompanyId` nvarchar(450) NOT NULL DEFAULT '';

ALTER TABLE `QuotationDetails` ADD `CompanyId` nvarchar(450) NOT NULL DEFAULT '';

ALTER TABLE `Quotation` ADD `CompanyId` nvarchar(450) NOT NULL DEFAULT '';

ALTER TABLE `Products` ADD `CompanyId` nvarchar(450) NOT NULL DEFAULT '';

ALTER TABLE `AspNetUsers` ADD `ProfilePicture` LONGBLOB NULL;

CREATE TABLE `Company` (
    `CompanyId` nvarchar(450) NOT NULL,
    `CompanyName` nvarchar(100) NOT NULL,
    `CompanyCreationDate` datetime(6) NOT NULL,
    `CompanyLogoImg` longblob NULL,
    `GSTIN` longtext NOT NULL,
    `BankName` nvarchar(100) NOT NULL,
    `BankAccountType` int NOT NULL,
    `AccountNumber` longtext NOT NULL,
    `IFSCcode` longtext NOT NULL,
    `PAN` longtext NOT NULL,
    `CompanyOwner` longtext NULL,
    PRIMARY KEY (`CompanyId`)
);

CREATE INDEX `IX_SpecialCharges_CompanyId` ON `SpecialCharges` (`CompanyId`);

CREATE INDEX `IX_QuotationNote_CompanyId` ON `QuotationNote` (`CompanyId`);

CREATE INDEX `IX_Quotation_CompanyId` ON `Quotation` (`CompanyId`);

CREATE INDEX `IX_Products_CompanyId` ON `Products` (`CompanyId`);

ALTER TABLE `Products` ADD CONSTRAINT `FK_Products_Company_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Company` (`CompanyId`) ON DELETE CASCADE;

ALTER TABLE `Quotation` ADD CONSTRAINT `FK_Quotation_Company_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Company` (`CompanyId`) ON DELETE CASCADE;

ALTER TABLE `QuotationNote` ADD CONSTRAINT `FK_QuotationNote_Company_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Company` (`CompanyId`) ON DELETE CASCADE;

ALTER TABLE `SpecialCharges` ADD CONSTRAINT `FK_SpecialCharges_Company_CompanyId` FOREIGN KEY (`CompanyId`) REFERENCES `Company` (`CompanyId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200723073806_ProfilePhotoAndCompanyAdded', '3.1.6');

ALTER TABLE `SpecialCharges` DROP COLUMN `SpecialChargeAmount`;

ALTER TABLE `SpecialCharges` ADD `SpecialChargeFixedAmount` real NOT NULL DEFAULT 0;

ALTER TABLE `SpecialCharges` ADD `SpecialChargePercentage` real NULL;

ALTER TABLE `SpecialCharges` ADD `SpecialChargeType` int NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200724073031_SpecialChargesModifications', '3.1.6');

ALTER TABLE `QuotationNote` ADD `NoteName` nvarchar(200) NOT NULL DEFAULT '';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200724103044_QuotationNoteNameAdded', '3.1.6');

ALTER TABLE `Quotation` DROP CONSTRAINT `FK_Quotation_AspNetUsers_CreatedByUserId`;

DROP INDEX IX_Quotation_CreatedByUserId ON Quotation;

ALTER TABLE Quotation MODIFY `CreatedByUserId` nvarchar(200) NOT NULL;

ALTER TABLE `Quotation` ADD `QuotationForId` nvarchar(450) NOT NULL DEFAULT '';

CREATE INDEX `IX_Quotation_QuotationForId` ON `Quotation` (`QuotationForId`);

ALTER TABLE `Quotation` ADD CONSTRAINT `FK_Quotation_AspNetUsers_QuotationForId` FOREIGN KEY (`QuotationForId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200725064848_QuotationForIdAdded', '3.1.6');

ALTER TABLE `Products` ADD `ProductDimensionX` real NOT NULL DEFAULT 0;

ALTER TABLE `Products` ADD `ProductDimensionY` real NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200730172946_AddedDefaultDimensionsProduct', '3.1.6');

ALTER TABLE `Quotation` ADD `QuotationNumber` int NOT NULL AUTO_INCREMENT UNIQUE;

CREATE UNIQUE INDEX `IX_Quotation_QuotationNumber` ON `Quotation` (`QuotationNumber`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200807054848_QuotationNumberAdded', '3.1.6');

ALTER TABLE `Quotation` ADD `SecretCode` int NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200808023108_SecretCodeAdded', '3.1.6');

ALTER TABLE Quotation MODIFY `SecretCode` text NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200808023823_SecretCodeString', '3.1.6');

ALTER TABLE `Company` ADD `CompanyAddress` nvarchar(200) NOT NULL DEFAULT '';

ALTER TABLE `Company` ADD `CompanyEmail` nvarchar(200) NOT NULL DEFAULT '';

ALTER TABLE `Company` ADD `CompanyPhoneNumber` nvarchar(10) NOT NULL DEFAULT '';

ALTER TABLE `AspNetUsers` ADD `PhysicalAddress` nvarchar(200) NOT NULL DEFAULT '';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200808070210_CompanyInfoUpdated', '3.1.6');

ALTER TABLE `Quotation` ADD `QuotationStatus` int NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200810131357_QuotationStatus', '3.1.6');

ALTER TABLE `Quotation` ADD `ActualQuotationCreationDate` datetime NOT NULL DEFAULT '0001-01-01 00:00:00.000000';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200815161037_QuotationCreationDateAdded', '3.1.6');

ALTER TABLE `QuotationNote` ADD `NoteCreationDate` datetime NOT NULL DEFAULT '0001-01-01 00:00:00.000000';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200818074403_NoteCreationDateAdded', '3.1.6');

ALTER TABLE `QuotationSpecialCharges` ADD `SpecialChargeAmount` real NOT NULL DEFAULT 0;

ALTER TABLE `Quotation` ADD `QuotationGrandTotalAmount` real NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200820055614_GrandTotalAdded', '3.1.6');

ALTER TABLE `QuotationSpecialCharges` ADD `DefaultCalculationOverride` bit NOT NULL DEFAULT FALSE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200821084218_DefaultCalcOverride', '3.1.6');

DROP INDEX UserNameIndex ON AspNetUsers;

DROP INDEX RoleNameIndex ON AspNetRoles;

ALTER TABLE SpecialCharges MODIFY `SpecialChargePercentage` float NOT NULL;

ALTER TABLE SpecialCharges MODIFY `SpecialChargeName` text NOT NULL;

ALTER TABLE SpecialCharges MODIFY `SpecialChargeFixedAmount` float NOT NULL;

ALTER TABLE SpecialCharges MODIFY `CompanyId` nvarchar(450) NOT NULL;

ALTER TABLE SpecialCharges MODIFY `SpecialChargesId` nvarchar(450) NOT NULL;

ALTER TABLE QuotationSpecialCharges MODIFY `SpecialChargeAmount` float NOT NULL;

ALTER TABLE QuotationSpecialCharges MODIFY `CompanyId` NVARCHAR(450) NOT NULL;

ALTER TABLE QuotationSpecialCharges MODIFY `QuotationId` nvarchar(450) NOT NULL;

ALTER TABLE QuotationSpecialCharges MODIFY `SpecialChargesId` nvarchar(450) NOT NULL;

ALTER TABLE QuotationNote MODIFY `NoteName` text NOT NULL;

ALTER TABLE QuotationNote MODIFY `NoteCreationDate` datetime NOT NULL;

ALTER TABLE QuotationNote MODIFY `Note` text NOT NULL;

ALTER TABLE QuotationNote MODIFY `CompanyId` nvarchar(450) NOT NULL;

ALTER TABLE QuotationNote MODIFY `QuotationNoteId` nvarchar(450) NOT NULL;

ALTER TABLE QuotationDetails MODIFY `TotalArea` float NOT NULL;

ALTER TABLE QuotationDetails MODIFY `QuotationId` nvarchar(450) NOT NULL;

ALTER TABLE QuotationDetails MODIFY `ProductRate` float NOT NULL;

ALTER TABLE QuotationDetails MODIFY `ProductName` text NOT NULL;

ALTER TABLE QuotationDetails MODIFY `ProductId` nvarchar(450) NOT NULL;

ALTER TABLE QuotationDetails MODIFY `ProductDimensionY` float NOT NULL;

ALTER TABLE QuotationDetails MODIFY `ProductDimensionX` float NOT NULL;

ALTER TABLE QuotationDetails MODIFY `ProductAmount` float NOT NULL;

ALTER TABLE QuotationDetails MODIFY `CompanyId` nvarchar(450) NOT NULL;

ALTER TABLE QuotationDetails MODIFY `QuotationDetailsId` nvarchar(450) NOT NULL;

ALTER TABLE Quotation MODIFY `SecretCode` text NOT NULL;

ALTER TABLE Quotation MODIFY `QuotationTo` text NOT NULL;

ALTER TABLE Quotation MODIFY `QuotationNoteId` nvarchar(450) NOT NULL;

ALTER TABLE Quotation MODIFY `QuotationModificationDate` datetime NOT NULL;

ALTER TABLE Quotation MODIFY `QuotationGrandTotalAmount` float NOT NULL;

ALTER TABLE Quotation MODIFY `QuotationForId` nvarchar(450) NOT NULL;

ALTER TABLE Quotation MODIFY `QuotationCreationDate` datetime NOT NULL;

ALTER TABLE Quotation MODIFY `QuotationAmount` float NOT NULL;

ALTER TABLE Quotation MODIFY `ModifiedByUserId` NVARCHAR(450) NULL;

ALTER TABLE Quotation MODIFY `CreatedByUserId` NVARCHAR(450) NOT NULL;

ALTER TABLE Quotation MODIFY `CompanyId` nvarchar(450) NOT NULL;

ALTER TABLE Quotation MODIFY `ActualQuotationCreationDate` datetime NOT NULL;

ALTER TABLE Quotation MODIFY `QuotationId` nvarchar(450) NOT NULL;

ALTER TABLE Products MODIFY `ProductPieceCutPrice` float NOT NULL;

ALTER TABLE Products MODIFY `ProductName` text NOT NULL;

ALTER TABLE Products MODIFY `ProductModificationDate` datetime NOT NULL;

ALTER TABLE Products MODIFY `ProductFullSheetPrice` float NOT NULL;

ALTER TABLE Products MODIFY `ProductDimensionY` float NOT NULL;

ALTER TABLE Products MODIFY `ProductDimensionX` float NOT NULL;

ALTER TABLE Products MODIFY `ProductCreationDate` datetime NOT NULL;

ALTER TABLE Products MODIFY `ModifiedByUserId` NVARCHAR(450) NULL;

ALTER TABLE Products MODIFY `CreatedByUserId` nvarchar(450) NOT NULL;

ALTER TABLE Products MODIFY `CompanyId` nvarchar(450) NOT NULL;

ALTER TABLE Products MODIFY `ProductId` nvarchar(450) NOT NULL;

ALTER TABLE Company MODIFY `PAN` text NOT NULL;

ALTER TABLE Company MODIFY `IFSCcode` text NOT NULL;

ALTER TABLE Company MODIFY `GSTIN` text NOT NULL;

ALTER TABLE Company MODIFY `CompanyPhoneNumber` text NOT NULL;

ALTER TABLE Company MODIFY `CompanyOwner` text NOT NULL;

ALTER TABLE Company MODIFY `CompanyName` text NOT NULL;

ALTER TABLE Company MODIFY `CompanyLogoImg` longblob NOT NULL;

ALTER TABLE Company MODIFY `CompanyEmail` text NOT NULL;

ALTER TABLE Company MODIFY `CompanyCreationDate` datetime NOT NULL;

ALTER TABLE Company MODIFY `CompanyAddress` text NOT NULL;

ALTER TABLE Company MODIFY `BankName` text NOT NULL;

ALTER TABLE Company MODIFY `AccountNumber` text NOT NULL;

ALTER TABLE Company MODIFY `CompanyId` nvarchar(450) NOT NULL;

ALTER TABLE AspNetUserTokens MODIFY `Value` text NOT NULL;

ALTER TABLE AspNetUserTokens MODIFY `Name` varchar(128) NOT NULL;

ALTER TABLE AspNetUserTokens MODIFY `LoginProvider` varchar(128) NOT NULL;

ALTER TABLE AspNetUserTokens MODIFY `UserId` nvarchar(450) NOT NULL;

ALTER TABLE AspNetUsers MODIFY `UserName` text NOT NULL;

ALTER TABLE AspNetUsers MODIFY `SecurityStamp` text NOT NULL;

ALTER TABLE AspNetUsers MODIFY `ProfilePicture` LONGBLOB NULL;

ALTER TABLE AspNetUsers MODIFY `PhysicalAddress` text NOT NULL;

ALTER TABLE AspNetUsers MODIFY `PhoneNumber` text NOT NULL;

ALTER TABLE AspNetUsers MODIFY `PasswordHash` text NOT NULL;

ALTER TABLE AspNetUsers MODIFY `NormalizedUserName` nvarchar(256) NOT NULL;

ALTER TABLE AspNetUsers MODIFY `NormalizedEmail` nvarchar(256) NOT NULL;

ALTER TABLE AspNetUsers MODIFY `Name` text NOT NULL;

ALTER TABLE AspNetUsers MODIFY `LockoutEnd` datetime(6) NULL;

ALTER TABLE AspNetUsers MODIFY `Email` text NOT NULL;

ALTER TABLE AspNetUsers MODIFY `ConcurrencyStamp` text NOT NULL;

ALTER TABLE AspNetUsers MODIFY `Id` nvarchar(450) NOT NULL;

ALTER TABLE AspNetUserRoles MODIFY `RoleId` nvarchar(450) NOT NULL;

ALTER TABLE AspNetUserRoles MODIFY `UserId` nvarchar(450) NOT NULL;

ALTER TABLE AspNetUserLogins MODIFY `UserId` nvarchar(450) NOT NULL;

ALTER TABLE AspNetUserLogins MODIFY `ProviderDisplayName` text NOT NULL;

ALTER TABLE AspNetUserLogins MODIFY `ProviderKey` nvarchar(128) NOT NULL;

ALTER TABLE AspNetUserLogins MODIFY `LoginProvider` nvarchar(128) NOT NULL;

ALTER TABLE AspNetUserClaims MODIFY `UserId` nvarchar(450) NOT NULL;

ALTER TABLE AspNetUserClaims MODIFY `ClaimValue` text NOT NULL;

ALTER TABLE AspNetUserClaims MODIFY `ClaimType` text NOT NULL;

ALTER TABLE AspNetRoles MODIFY `NormalizedName` NVARCHAR(256) NOT NULL;

ALTER TABLE AspNetRoles MODIFY `Name` text NOT NULL;

ALTER TABLE AspNetRoles MODIFY `ConcurrencyStamp` text NOT NULL;

ALTER TABLE AspNetRoles MODIFY `Id` nvarchar(450) NOT NULL;

ALTER TABLE AspNetRoleClaims MODIFY `RoleId` nvarchar(450) NOT NULL;

ALTER TABLE AspNetRoleClaims MODIFY `ClaimValue` text NOT NULL;

ALTER TABLE AspNetRoleClaims MODIFY `ClaimType` text NOT NULL;

CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);

CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200912192354_MySQLSupport', '3.1.6');

