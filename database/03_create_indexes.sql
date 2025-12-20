-- =====================================================
-- PKT Database Indexes Creation Script
-- =====================================================

-- Indexes for PktTransactions
CREATE INDEX IF NOT EXISTS "IX_PktTransactions_DelayReasonId" ON "PktTransactions" ("DelayReasonId");
CREATE INDEX IF NOT EXISTS "IX_PktTransactions_ReaktorId" ON "PktTransactions" ("ReactorId");
CREATE INDEX IF NOT EXISTS "IX_PktTransactions_ProductId" ON "PktTransactions" ("ProductId");
CREATE INDEX IF NOT EXISTS "IX_PktTransactions_Status" ON "PktTransactions" ("Status");
CREATE INDEX IF NOT EXISTS "IX_PktTransactions_WorkOrderNo" ON "PktTransactions" ("WorkOrderNo");
CREATE INDEX IF NOT EXISTS "IX_PktTransactions_LotNo" ON "PktTransactions" ("LotNo");
CREATE INDEX IF NOT EXISTS "IX_PktTransactions_CreatedAt" ON "PktTransactions" ("CreatedAt" DESC);

-- Indexes for Products
CREATE INDEX IF NOT EXISTS "IX_Products_ProductCode" ON "Products" ("ProductCode");
CREATE INDEX IF NOT EXISTS "IX_Products_SBU" ON "Products" ("SBU");

-- Indexes for Reactors
CREATE INDEX IF NOT EXISTS "IX_Reactors_Name" ON "Reactors" ("Name");

-- Indexes for DelayReasons
CREATE INDEX IF NOT EXISTS "IX_DelayReasons_Name" ON "DelayReasons" ("Name");

-- Indexes for ApplicationLogs
CREATE INDEX IF NOT EXISTS "IX_ApplicationLogs_Timestamp" ON "ApplicationLogs" ("Timestamp" DESC);
CREATE INDEX IF NOT EXISTS "IX_ApplicationLogs_Level" ON "ApplicationLogs" ("Level");
CREATE INDEX IF NOT EXISTS "IX_ApplicationLogs_Logger" ON "ApplicationLogs" ("Logger");
