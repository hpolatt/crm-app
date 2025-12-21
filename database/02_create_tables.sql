-- =====================================================
-- PKT Database Tables Creation Script
-- =====================================================
-- This script creates all tables for the PKT application
-- =====================================================

-- Create Users table
CREATE TABLE "Users" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "Username" character varying(100) NOT NULL UNIQUE,
    "PasswordHash" character varying(255) NOT NULL,
    "FirstName" character varying(100) NOT NULL,
    "LastName" character varying(100) NOT NULL,
    "Email" character varying(255) NOT NULL,
    "Role" VARCHAR(50) NOT NULL DEFAULT 'Foreman',
    "IsActive" boolean NOT NULL DEFAULT true,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" timestamp with time zone NULL,
    
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

-- Create index on Username for fast lookup
CREATE INDEX "IX_Users_Username" ON "Users" ("Username");


-- Create DelayReasons table
CREATE TABLE IF NOT EXISTS "DelayReasons" (
    "Id" uuid PRIMARY KEY,
    "Name" character varying(255) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL
);

-- Create Reactors table
CREATE TABLE IF NOT EXISTS "Reactors" (
    "Id" uuid PRIMARY KEY,
    "Name" character varying(255) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL
);

-- Create Products table
CREATE TABLE IF NOT EXISTS "Products" (
    "Id" uuid PRIMARY KEY,
    "SBU" character varying(100) NOT NULL,
    "ProductCode" character varying(100) NOT NULL,
    "ProductName" character varying(255) NOT NULL,
    "MinProductionQuantity" numeric(18,2) NOT NULL,
    "MaxProductionQuantity" numeric(18,2) NOT NULL,
    "ProductionDurationMinutes" integer NOT NULL,
    "Notes" text NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL
);

-- Create PktTransactions table
CREATE TABLE IF NOT EXISTS "PktTransactions" (
    "Id" uuid PRIMARY KEY,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Planned',
    "ReactorId" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "WorkOrderNo" character varying(100) NOT NULL,
    "LotNo" character varying(100) NOT NULL,
    "StartOfWork" timestamp with time zone NULL,
    "End" timestamp with time zone NULL,
    "ActualProductionDuration" interval NULL,
    "DelayDuration" interval NULL,
    "WashingDuration" interval NULL,
    "CausticAmountKg" numeric(18,2) NULL,
    "DelayReasonId" uuid NULL,
    "Description" text NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    
    CONSTRAINT "FK_PktTransactions_DelayReasons_DelayReasonId" 
        FOREIGN KEY ("DelayReasonId") REFERENCES "DelayReasons" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_PktTransactions_Reactors_ReactorId" 
        FOREIGN KEY ("ReactorId") REFERENCES "Reactors" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_PktTransactions_Products_ProductId" 
        FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE RESTRICT
);

-- Create ApplicationLogs table for storing application logs
CREATE TABLE IF NOT EXISTS "ApplicationLogs" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "Timestamp" timestamp with time zone NOT NULL DEFAULT NOW(),
    "Level" character varying(50) NOT NULL,
    "Message" text NOT NULL,
    "Exception" text NULL,
    "Logger" character varying(255) NULL,
    "RequestPath" character varying(500) NULL,
    "RequestMethod" character varying(10) NULL,
    "StatusCode" integer NULL,
    "UserName" character varying(255) NULL,
    "MachineName" character varying(255) NULL,
    "Properties" jsonb NULL
);
