-- Script Date: 23/10/2024 10:27  - ErikEJ.SqlCeScripting version 3.5.2.95
CREATE TABLE [Product] (
  [ProductId] INTEGER NOT NULL
, [SKU] TEXT NOT NULL
, [Name] TEXT NOT NULL
, [Colour] TEXT NULL
, [Description] TEXT NULL
, CONSTRAINT [PK_Product] PRIMARY KEY ([ProductId])
);

CREATE UNIQUE INDEX UN_Product_SKU ON Product (SKU);