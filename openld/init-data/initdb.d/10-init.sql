﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "AspNetRoles" (
    "Id" text NOT NULL,
    "Name" character varying(256) NULL,
    "NormalizedName" character varying(256) NULL,
    "ConcurrencyStamp" text NULL,
    CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetUsers" (
    "Id" text NOT NULL,
    "UserName" character varying(256) NULL,
    "NormalizedUserName" character varying(256) NULL,
    "Email" character varying(256) NULL,
    "NormalizedEmail" character varying(256) NULL,
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text NULL,
    "SecurityStamp" text NULL,
    "ConcurrencyStamp" text NULL,
    "PhoneNumber" text NULL,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone NULL,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL,
    CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id")
);

CREATE TABLE "DeviceCodes" (
    "UserCode" character varying(200) NOT NULL,
    "DeviceCode" character varying(200) NOT NULL,
    "SubjectId" character varying(200) NULL,
    "ClientId" character varying(200) NOT NULL,
    "CreationTime" timestamp without time zone NOT NULL,
    "Expiration" timestamp without time zone NOT NULL,
    "Data" character varying(50000) NOT NULL,
    CONSTRAINT "PK_DeviceCodes" PRIMARY KEY ("UserCode")
);

CREATE TABLE "FixtureTypes" (
    "Id" text NOT NULL,
    "Name" text NULL,
    CONSTRAINT "PK_FixtureTypes" PRIMARY KEY ("Id")
);

CREATE TABLE "PersistedGrants" (
    "Key" character varying(200) NOT NULL,
    "Type" character varying(50) NOT NULL,
    "SubjectId" character varying(200) NULL,
    "ClientId" character varying(200) NOT NULL,
    "CreationTime" timestamp without time zone NOT NULL,
    "Expiration" timestamp without time zone NULL,
    "Data" character varying(50000) NOT NULL,
    CONSTRAINT "PK_PersistedGrants" PRIMARY KEY ("Key")
);

CREATE TABLE "StoredImages" (
    "Id" text NOT NULL,
    "Hash" text NULL,
    "Path" text NULL,
    CONSTRAINT "PK_StoredImages" PRIMARY KEY ("Id")
);

CREATE TABLE "StructureTypes" (
    "Id" text NOT NULL,
    "Name" text NULL,
    CONSTRAINT "PK_StructureTypes" PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "RoleId" text NOT NULL,
    "ClaimType" text NULL,
    "ClaimValue" text NULL,
    CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "UserId" text NOT NULL,
    "ClaimType" text NULL,
    "ClaimValue" text NULL,
    CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" character varying(128) NOT NULL,
    "ProviderKey" character varying(128) NOT NULL,
    "ProviderDisplayName" text NULL,
    "UserId" text NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" character varying(128) NOT NULL,
    "Name" character varying(128) NOT NULL,
    "Value" text NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Drawings" (
    "Id" text NOT NULL,
    "Title" text NULL,
    "OwnerId" text NULL,
    "LastModified" timestamp without time zone NOT NULL,
    CONSTRAINT "PK_Drawings" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Drawings_AspNetUsers_OwnerId" FOREIGN KEY ("OwnerId") REFERENCES "AspNetUsers" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Fixtures" (
    "Id" text NOT NULL,
    "Name" text NULL,
    "Manufacturer" text NULL,
    "ReleaseDate" timestamp without time zone NOT NULL,
    "TypeId" text NULL,
    "Power" integer NOT NULL,
    "Weight" real NOT NULL,
    "Symbol" xml NULL,
    "ImageId" text NULL,
    CONSTRAINT "PK_Fixtures" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Fixtures_StoredImages_ImageId" FOREIGN KEY ("ImageId") REFERENCES "StoredImages" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Fixtures_FixtureTypes_TypeId" FOREIGN KEY ("TypeId") REFERENCES "FixtureTypes" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "UserDrawings" (
    "Id" text NOT NULL,
    "UserId" text NULL,
    "DrawingId" text NULL,
    CONSTRAINT "PK_UserDrawings" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_UserDrawings_Drawings_DrawingId" FOREIGN KEY ("DrawingId") REFERENCES "Drawings" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_UserDrawings_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Views" (
    "Id" text NOT NULL,
    "DrawingId" text NULL,
    "Name" text NULL,
    "Type" integer NOT NULL,
    CONSTRAINT "PK_Views" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Views_Drawings_DrawingId" FOREIGN KEY ("DrawingId") REFERENCES "Drawings" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "FixtureModes" (
    "Id" text NOT NULL,
    "Name" text NULL,
    "FixtureId" text NULL,
    "Addresses" text[] NULL,
    CONSTRAINT "PK_FixtureModes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_FixtureModes_Fixtures_FixtureId" FOREIGN KEY ("FixtureId") REFERENCES "Fixtures" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Structures" (
    "Id" text NOT NULL,
    "ViewId" text NULL,
    "Geometry" jsonb NULL,
    "Name" text NULL,
    "Rating" real NOT NULL,
    "TypeId" text NULL,
    "Notes" text NULL,
    CONSTRAINT "PK_Structures" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Structures_StructureTypes_TypeId" FOREIGN KEY ("TypeId") REFERENCES "StructureTypes" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Structures_Views_ViewId" FOREIGN KEY ("ViewId") REFERENCES "Views" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "RiggedFixtures" (
    "Id" text NOT NULL,
    "FixtureId" text NULL,
    "StructureId" text NULL,
    "Position" jsonb NULL,
    "Address" smallint NOT NULL,
    "Universe" smallint NOT NULL,
    "ModeId" text NULL,
    "Notes" text NULL,
    CONSTRAINT "PK_RiggedFixtures" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_RiggedFixtures_Fixtures_FixtureId" FOREIGN KEY ("FixtureId") REFERENCES "Fixtures" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_RiggedFixtures_FixtureModes_ModeId" FOREIGN KEY ("ModeId") REFERENCES "FixtureModes" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_RiggedFixtures_Structures_StructureId" FOREIGN KEY ("StructureId") REFERENCES "Structures" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

CREATE UNIQUE INDEX "IX_DeviceCodes_DeviceCode" ON "DeviceCodes" ("DeviceCode");

CREATE INDEX "IX_DeviceCodes_Expiration" ON "DeviceCodes" ("Expiration");

CREATE INDEX "IX_Drawings_OwnerId" ON "Drawings" ("OwnerId");

CREATE INDEX "IX_FixtureModes_FixtureId" ON "FixtureModes" ("FixtureId");

CREATE INDEX "IX_Fixtures_ImageId" ON "Fixtures" ("ImageId");

CREATE INDEX "IX_Fixtures_TypeId" ON "Fixtures" ("TypeId");

CREATE INDEX "IX_PersistedGrants_Expiration" ON "PersistedGrants" ("Expiration");

CREATE INDEX "IX_PersistedGrants_SubjectId_ClientId_Type" ON "PersistedGrants" ("SubjectId", "ClientId", "Type");

CREATE INDEX "IX_RiggedFixtures_FixtureId" ON "RiggedFixtures" ("FixtureId");

CREATE INDEX "IX_RiggedFixtures_ModeId" ON "RiggedFixtures" ("ModeId");

CREATE INDEX "IX_RiggedFixtures_StructureId" ON "RiggedFixtures" ("StructureId");

CREATE INDEX "IX_Structures_TypeId" ON "Structures" ("TypeId");

CREATE INDEX "IX_Structures_ViewId" ON "Structures" ("ViewId");

CREATE INDEX "IX_UserDrawings_DrawingId" ON "UserDrawings" ("DrawingId");

CREATE INDEX "IX_UserDrawings_UserId" ON "UserDrawings" ("UserId");

CREATE INDEX "IX_Views_DrawingId" ON "Views" ("DrawingId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20191228112834_RemovePostGIS', '3.1.0');

ALTER TABLE "Drawings" ADD "Height" real NOT NULL DEFAULT 0;

ALTER TABLE "Drawings" ADD "Width" real NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200102132022_DrawingSize', '3.1.0');

ALTER TABLE "Drawings" DROP COLUMN "Height";

ALTER TABLE "Drawings" DROP COLUMN "Width";

ALTER TABLE "Views" ADD "Height" real NOT NULL DEFAULT 0;

ALTER TABLE "Views" ADD "Width" real NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200106093404_ViewSize', '3.1.0');

ALTER TABLE "RiggedFixtures" DROP CONSTRAINT "FK_RiggedFixtures_Structures_StructureId";

ALTER TABLE "Structures" DROP CONSTRAINT "FK_Structures_Views_ViewId";

ALTER TABLE "Views" DROP CONSTRAINT "FK_Views_Drawings_DrawingId";

ALTER TABLE "Views" ALTER COLUMN "DrawingId" TYPE text;
ALTER TABLE "Views" ALTER COLUMN "DrawingId" SET NOT NULL;
ALTER TABLE "Views" ALTER COLUMN "DrawingId" DROP DEFAULT;

ALTER TABLE "Structures" ALTER COLUMN "ViewId" TYPE text;
ALTER TABLE "Structures" ALTER COLUMN "ViewId" SET NOT NULL;
ALTER TABLE "Structures" ALTER COLUMN "ViewId" DROP DEFAULT;

ALTER TABLE "RiggedFixtures" ALTER COLUMN "StructureId" TYPE text;
ALTER TABLE "RiggedFixtures" ALTER COLUMN "StructureId" SET NOT NULL;
ALTER TABLE "RiggedFixtures" ALTER COLUMN "StructureId" DROP DEFAULT;

ALTER TABLE "RiggedFixtures" ADD CONSTRAINT "FK_RiggedFixtures_Structures_StructureId" FOREIGN KEY ("StructureId") REFERENCES "Structures" ("Id") ON DELETE CASCADE;

ALTER TABLE "Structures" ADD CONSTRAINT "FK_Structures_Views_ViewId" FOREIGN KEY ("ViewId") REFERENCES "Views" ("Id") ON DELETE CASCADE;

ALTER TABLE "Views" ADD CONSTRAINT "FK_Views_Drawings_DrawingId" FOREIGN KEY ("DrawingId") REFERENCES "Drawings" ("Id") ON DELETE CASCADE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200106112912_CascadeDelete', '3.1.0');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200117155630_FixtureModes', '3.1.0');

ALTER TABLE "FixtureModes" DROP COLUMN "Addresses";

ALTER TABLE "FixtureModes" ADD "Channels" text[] NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200117160041_FixtureModeChannels', '3.1.0');

ALTER TABLE "FixtureModes" DROP CONSTRAINT "FK_FixtureModes_Fixtures_FixtureId";

ALTER TABLE "UserDrawings" DROP CONSTRAINT "FK_UserDrawings_Drawings_DrawingId";

ALTER TABLE "UserDrawings" ALTER COLUMN "DrawingId" TYPE text;
ALTER TABLE "UserDrawings" ALTER COLUMN "DrawingId" SET NOT NULL;
ALTER TABLE "UserDrawings" ALTER COLUMN "DrawingId" DROP DEFAULT;

ALTER TABLE "FixtureModes" ALTER COLUMN "FixtureId" TYPE text;
ALTER TABLE "FixtureModes" ALTER COLUMN "FixtureId" SET NOT NULL;
ALTER TABLE "FixtureModes" ALTER COLUMN "FixtureId" DROP DEFAULT;

ALTER TABLE "FixtureModes" ADD CONSTRAINT "FK_FixtureModes_Fixtures_FixtureId" FOREIGN KEY ("FixtureId") REFERENCES "Fixtures" ("Id") ON DELETE CASCADE;

ALTER TABLE "UserDrawings" ADD CONSTRAINT "FK_UserDrawings_Drawings_DrawingId" FOREIGN KEY ("DrawingId") REFERENCES "Drawings" ("Id") ON DELETE CASCADE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200117161619_CascadeModesUserDrawing', '3.1.0');

ALTER TABLE "RiggedFixtures" ADD "Angle" integer NOT NULL DEFAULT 0;

ALTER TABLE "RiggedFixtures" ADD "Colour" text NULL;

ALTER TABLE "RiggedFixtures" ADD "Name" text NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200122153221_RiggedFixture_Name-Colour-Angle', '3.1.0');

ALTER TABLE "Fixtures" DROP COLUMN "Symbol";

ALTER TABLE "Fixtures" ADD "SymbolId" text NULL;

CREATE TABLE "Symbols" (
    "Id" text NOT NULL,
    "Hash" text NULL,
    "Path" text NULL,
    "BitmapId" text NULL,
    CONSTRAINT "PK_Symbols" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Symbols_StoredImages_BitmapId" FOREIGN KEY ("BitmapId") REFERENCES "StoredImages" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_Fixtures_SymbolId" ON "Fixtures" ("SymbolId");

CREATE INDEX "IX_Symbols_BitmapId" ON "Symbols" ("BitmapId");

ALTER TABLE "Fixtures" ADD CONSTRAINT "FK_Fixtures_Symbols_SymbolId" FOREIGN KEY ("SymbolId") REFERENCES "Symbols" ("Id") ON DELETE RESTRICT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200125164625_FixtureSymbolBitmaps', '3.1.0');

CREATE TABLE "Templates" (
    "Id" text NOT NULL,
    "DrawingId" text NULL,
    CONSTRAINT "PK_Templates" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Templates_Drawings_DrawingId" FOREIGN KEY ("DrawingId") REFERENCES "Drawings" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_Templates_DrawingId" ON "Templates" ("DrawingId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200125200025_DrawingTemplates', '3.1.0');

