--
-- PostgreSQL database dump
--

-- Dumped from database version 12.1 (Debian 12.1-1.pgdg100+1)
-- Dumped by pg_dump version 12.1 (Debian 12.1-1.pgdg100+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: AspNetRoleClaims; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."AspNetRoleClaims" (
    "Id" integer NOT NULL,
    "RoleId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetRoleClaims" OWNER TO openld;

--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: openld
--

ALTER TABLE public."AspNetRoleClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetRoleClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetRoles; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."AspNetRoles" (
    "Id" text NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "ConcurrencyStamp" text
);


ALTER TABLE public."AspNetRoles" OWNER TO openld;

--
-- Name: AspNetUserClaims; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."AspNetUserClaims" (
    "Id" integer NOT NULL,
    "UserId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetUserClaims" OWNER TO openld;

--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: openld
--

ALTER TABLE public."AspNetUserClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetUserClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetUserLogins; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."AspNetUserLogins" (
    "LoginProvider" character varying(128) NOT NULL,
    "ProviderKey" character varying(128) NOT NULL,
    "ProviderDisplayName" text,
    "UserId" text NOT NULL
);


ALTER TABLE public."AspNetUserLogins" OWNER TO openld;

--
-- Name: AspNetUserRoles; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL
);


ALTER TABLE public."AspNetUserRoles" OWNER TO openld;

--
-- Name: AspNetUserTokens; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" character varying(128) NOT NULL,
    "Name" character varying(128) NOT NULL,
    "Value" text
);


ALTER TABLE public."AspNetUserTokens" OWNER TO openld;

--
-- Name: AspNetUsers; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."AspNetUsers" (
    "Id" text NOT NULL,
    "UserName" character varying(256),
    "NormalizedUserName" character varying(256),
    "Email" character varying(256),
    "NormalizedEmail" character varying(256),
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "PhoneNumber" text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL
);


ALTER TABLE public."AspNetUsers" OWNER TO openld;

--
-- Name: DeviceCodes; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."DeviceCodes" (
    "UserCode" character varying(200) NOT NULL,
    "DeviceCode" character varying(200) NOT NULL,
    "SubjectId" character varying(200),
    "ClientId" character varying(200) NOT NULL,
    "CreationTime" timestamp without time zone NOT NULL,
    "Expiration" timestamp without time zone NOT NULL,
    "Data" character varying(50000) NOT NULL
);


ALTER TABLE public."DeviceCodes" OWNER TO openld;

--
-- Name: Drawings; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."Drawings" (
    "Id" text NOT NULL,
    "Title" text,
    "OwnerId" text,
    "LastModified" timestamp without time zone NOT NULL
);


ALTER TABLE public."Drawings" OWNER TO openld;

--
-- Name: FixtureModes; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."FixtureModes" (
    "Id" text NOT NULL,
    "Name" text,
    "FixtureId" text NOT NULL,
    "Channels" integer
);


ALTER TABLE public."FixtureModes" OWNER TO openld;

--
-- Name: FixtureTypes; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."FixtureTypes" (
    "Id" text NOT NULL,
    "Name" text
);


ALTER TABLE public."FixtureTypes" OWNER TO openld;

--
-- Name: Fixtures; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."Fixtures" (
    "Id" text NOT NULL,
    "Name" text,
    "Manufacturer" text,
    "ReleaseDate" timestamp without time zone NOT NULL,
    "TypeId" text,
    "Power" integer NOT NULL,
    "Weight" real NOT NULL,
    "ImageId" text,
    "SymbolId" text
);


ALTER TABLE public."Fixtures" OWNER TO openld;

--
-- Name: PersistedGrants; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."PersistedGrants" (
    "Key" character varying(200) NOT NULL,
    "Type" character varying(50) NOT NULL,
    "SubjectId" character varying(200),
    "ClientId" character varying(200) NOT NULL,
    "CreationTime" timestamp without time zone NOT NULL,
    "Expiration" timestamp without time zone,
    "Data" character varying(50000) NOT NULL
);


ALTER TABLE public."PersistedGrants" OWNER TO openld;

--
-- Name: RiggedFixtures; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."RiggedFixtures" (
    "Id" text NOT NULL,
    "FixtureId" text,
    "StructureId" text NOT NULL,
    "Position" jsonb,
    "Address" smallint NOT NULL,
    "Universe" smallint NOT NULL,
    "ModeId" text,
    "Notes" text,
    "Angle" smallint NOT NULL,
    "Colour" text,
    "Name" text,
    "Channel" smallint DEFAULT 0 NOT NULL
);


ALTER TABLE public."RiggedFixtures" OWNER TO openld;

--
-- Name: StoredImages; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."StoredImages" (
    "Id" text NOT NULL,
    "Hash" text,
    "Path" text
);


ALTER TABLE public."StoredImages" OWNER TO openld;

--
-- Name: StructureTypes; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."StructureTypes" (
    "Id" text NOT NULL,
    "Name" text
);


ALTER TABLE public."StructureTypes" OWNER TO openld;

--
-- Name: Structures; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."Structures" (
    "Id" text NOT NULL,
    "ViewId" text NOT NULL,
    "Geometry" jsonb,
    "Name" text,
    "Rating" real NOT NULL,
    "TypeId" text,
    "Notes" text
);


ALTER TABLE public."Structures" OWNER TO openld;

--
-- Name: Symbols; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."Symbols" (
    "Id" text NOT NULL,
    "Hash" text,
    "Path" text,
    "BitmapId" text
);


ALTER TABLE public."Symbols" OWNER TO openld;

--
-- Name: Templates; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."Templates" (
    "Id" text NOT NULL,
    "DrawingId" text
);


ALTER TABLE public."Templates" OWNER TO openld;

--
-- Name: UserDrawings; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."UserDrawings" (
    "Id" text NOT NULL,
    "UserId" text,
    "DrawingId" text NOT NULL
);


ALTER TABLE public."UserDrawings" OWNER TO openld;

--
-- Name: Views; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."Views" (
    "Id" text NOT NULL,
    "DrawingId" text NOT NULL,
    "Name" text,
    "Type" integer NOT NULL,
    "Height" real DEFAULT 0 NOT NULL,
    "Width" real DEFAULT 0 NOT NULL
);


ALTER TABLE public."Views" OWNER TO openld;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: openld
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO openld;

--
-- Name: AspNetRoleClaims PK_AspNetRoleClaims; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetRoles PK_AspNetRoles; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."AspNetRoles"
    ADD CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id");


--
-- Name: AspNetUserClaims PK_AspNetUserClaims; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetUserLogins PK_AspNetUserLogins; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey");


--
-- Name: AspNetUserRoles PK_AspNetUserRoles; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId");


--
-- Name: AspNetUserTokens PK_AspNetUserTokens; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name");


--
-- Name: AspNetUsers PK_AspNetUsers; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id");


--
-- Name: DeviceCodes PK_DeviceCodes; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."DeviceCodes"
    ADD CONSTRAINT "PK_DeviceCodes" PRIMARY KEY ("UserCode");


--
-- Name: Drawings PK_Drawings; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Drawings"
    ADD CONSTRAINT "PK_Drawings" PRIMARY KEY ("Id");


--
-- Name: FixtureModes PK_FixtureModes; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."FixtureModes"
    ADD CONSTRAINT "PK_FixtureModes" PRIMARY KEY ("Id");


--
-- Name: FixtureTypes PK_FixtureTypes; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."FixtureTypes"
    ADD CONSTRAINT "PK_FixtureTypes" PRIMARY KEY ("Id");


--
-- Name: Fixtures PK_Fixtures; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Fixtures"
    ADD CONSTRAINT "PK_Fixtures" PRIMARY KEY ("Id");


--
-- Name: PersistedGrants PK_PersistedGrants; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."PersistedGrants"
    ADD CONSTRAINT "PK_PersistedGrants" PRIMARY KEY ("Key");


--
-- Name: RiggedFixtures PK_RiggedFixtures; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."RiggedFixtures"
    ADD CONSTRAINT "PK_RiggedFixtures" PRIMARY KEY ("Id");


--
-- Name: StoredImages PK_StoredImages; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."StoredImages"
    ADD CONSTRAINT "PK_StoredImages" PRIMARY KEY ("Id");


--
-- Name: StructureTypes PK_StructureTypes; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."StructureTypes"
    ADD CONSTRAINT "PK_StructureTypes" PRIMARY KEY ("Id");


--
-- Name: Structures PK_Structures; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Structures"
    ADD CONSTRAINT "PK_Structures" PRIMARY KEY ("Id");


--
-- Name: Symbols PK_Symbols; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Symbols"
    ADD CONSTRAINT "PK_Symbols" PRIMARY KEY ("Id");


--
-- Name: Templates PK_Templates; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Templates"
    ADD CONSTRAINT "PK_Templates" PRIMARY KEY ("Id");


--
-- Name: UserDrawings PK_UserDrawings; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."UserDrawings"
    ADD CONSTRAINT "PK_UserDrawings" PRIMARY KEY ("Id");


--
-- Name: Views PK_Views; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Views"
    ADD CONSTRAINT "PK_Views" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: EmailIndex; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "EmailIndex" ON public."AspNetUsers" USING btree ("NormalizedEmail");


--
-- Name: IX_AspNetRoleClaims_RoleId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON public."AspNetRoleClaims" USING btree ("RoleId");


--
-- Name: IX_AspNetUserClaims_UserId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_AspNetUserClaims_UserId" ON public."AspNetUserClaims" USING btree ("UserId");


--
-- Name: IX_AspNetUserLogins_UserId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_AspNetUserLogins_UserId" ON public."AspNetUserLogins" USING btree ("UserId");


--
-- Name: IX_AspNetUserRoles_RoleId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON public."AspNetUserRoles" USING btree ("RoleId");


--
-- Name: IX_DeviceCodes_DeviceCode; Type: INDEX; Schema: public; Owner: openld
--

CREATE UNIQUE INDEX "IX_DeviceCodes_DeviceCode" ON public."DeviceCodes" USING btree ("DeviceCode");


--
-- Name: IX_DeviceCodes_Expiration; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_DeviceCodes_Expiration" ON public."DeviceCodes" USING btree ("Expiration");


--
-- Name: IX_Drawings_OwnerId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_Drawings_OwnerId" ON public."Drawings" USING btree ("OwnerId");


--
-- Name: IX_FixtureModes_FixtureId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_FixtureModes_FixtureId" ON public."FixtureModes" USING btree ("FixtureId");


--
-- Name: IX_Fixtures_ImageId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_Fixtures_ImageId" ON public."Fixtures" USING btree ("ImageId");


--
-- Name: IX_Fixtures_SymbolId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_Fixtures_SymbolId" ON public."Fixtures" USING btree ("SymbolId");


--
-- Name: IX_Fixtures_TypeId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_Fixtures_TypeId" ON public."Fixtures" USING btree ("TypeId");


--
-- Name: IX_PersistedGrants_Expiration; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_PersistedGrants_Expiration" ON public."PersistedGrants" USING btree ("Expiration");


--
-- Name: IX_PersistedGrants_SubjectId_ClientId_Type; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_PersistedGrants_SubjectId_ClientId_Type" ON public."PersistedGrants" USING btree ("SubjectId", "ClientId", "Type");


--
-- Name: IX_RiggedFixtures_FixtureId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_RiggedFixtures_FixtureId" ON public."RiggedFixtures" USING btree ("FixtureId");


--
-- Name: IX_RiggedFixtures_ModeId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_RiggedFixtures_ModeId" ON public."RiggedFixtures" USING btree ("ModeId");


--
-- Name: IX_RiggedFixtures_StructureId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_RiggedFixtures_StructureId" ON public."RiggedFixtures" USING btree ("StructureId");


--
-- Name: IX_Structures_TypeId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_Structures_TypeId" ON public."Structures" USING btree ("TypeId");


--
-- Name: IX_Structures_ViewId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_Structures_ViewId" ON public."Structures" USING btree ("ViewId");


--
-- Name: IX_Symbols_BitmapId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_Symbols_BitmapId" ON public."Symbols" USING btree ("BitmapId");


--
-- Name: IX_Templates_DrawingId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_Templates_DrawingId" ON public."Templates" USING btree ("DrawingId");


--
-- Name: IX_UserDrawings_DrawingId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_UserDrawings_DrawingId" ON public."UserDrawings" USING btree ("DrawingId");


--
-- Name: IX_UserDrawings_UserId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_UserDrawings_UserId" ON public."UserDrawings" USING btree ("UserId");


--
-- Name: IX_Views_DrawingId; Type: INDEX; Schema: public; Owner: openld
--

CREATE INDEX "IX_Views_DrawingId" ON public."Views" USING btree ("DrawingId");


--
-- Name: RoleNameIndex; Type: INDEX; Schema: public; Owner: openld
--

CREATE UNIQUE INDEX "RoleNameIndex" ON public."AspNetRoles" USING btree ("NormalizedName");


--
-- Name: UserNameIndex; Type: INDEX; Schema: public; Owner: openld
--

CREATE UNIQUE INDEX "UserNameIndex" ON public."AspNetUsers" USING btree ("NormalizedUserName");


--
-- Name: AspNetRoleClaims FK_AspNetRoleClaims_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserClaims FK_AspNetUserClaims_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserLogins FK_AspNetUserLogins_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserTokens FK_AspNetUserTokens_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: Drawings FK_Drawings_AspNetUsers_OwnerId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Drawings"
    ADD CONSTRAINT "FK_Drawings_AspNetUsers_OwnerId" FOREIGN KEY ("OwnerId") REFERENCES public."AspNetUsers"("Id") ON DELETE RESTRICT;


--
-- Name: FixtureModes FK_FixtureModes_Fixtures_FixtureId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."FixtureModes"
    ADD CONSTRAINT "FK_FixtureModes_Fixtures_FixtureId" FOREIGN KEY ("FixtureId") REFERENCES public."Fixtures"("Id") ON DELETE CASCADE;


--
-- Name: Fixtures FK_Fixtures_FixtureTypes_TypeId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Fixtures"
    ADD CONSTRAINT "FK_Fixtures_FixtureTypes_TypeId" FOREIGN KEY ("TypeId") REFERENCES public."FixtureTypes"("Id") ON DELETE RESTRICT;


--
-- Name: Fixtures FK_Fixtures_StoredImages_ImageId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Fixtures"
    ADD CONSTRAINT "FK_Fixtures_StoredImages_ImageId" FOREIGN KEY ("ImageId") REFERENCES public."StoredImages"("Id") ON DELETE RESTRICT;


--
-- Name: Fixtures FK_Fixtures_Symbols_SymbolId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Fixtures"
    ADD CONSTRAINT "FK_Fixtures_Symbols_SymbolId" FOREIGN KEY ("SymbolId") REFERENCES public."Symbols"("Id") ON DELETE RESTRICT;


--
-- Name: RiggedFixtures FK_RiggedFixtures_FixtureModes_ModeId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."RiggedFixtures"
    ADD CONSTRAINT "FK_RiggedFixtures_FixtureModes_ModeId" FOREIGN KEY ("ModeId") REFERENCES public."FixtureModes"("Id") ON DELETE RESTRICT;


--
-- Name: RiggedFixtures FK_RiggedFixtures_Fixtures_FixtureId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."RiggedFixtures"
    ADD CONSTRAINT "FK_RiggedFixtures_Fixtures_FixtureId" FOREIGN KEY ("FixtureId") REFERENCES public."Fixtures"("Id") ON DELETE RESTRICT;


--
-- Name: RiggedFixtures FK_RiggedFixtures_Structures_StructureId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."RiggedFixtures"
    ADD CONSTRAINT "FK_RiggedFixtures_Structures_StructureId" FOREIGN KEY ("StructureId") REFERENCES public."Structures"("Id") ON DELETE CASCADE;


--
-- Name: Structures FK_Structures_StructureTypes_TypeId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Structures"
    ADD CONSTRAINT "FK_Structures_StructureTypes_TypeId" FOREIGN KEY ("TypeId") REFERENCES public."StructureTypes"("Id") ON DELETE RESTRICT;


--
-- Name: Structures FK_Structures_Views_ViewId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Structures"
    ADD CONSTRAINT "FK_Structures_Views_ViewId" FOREIGN KEY ("ViewId") REFERENCES public."Views"("Id") ON DELETE CASCADE;


--
-- Name: Symbols FK_Symbols_StoredImages_BitmapId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Symbols"
    ADD CONSTRAINT "FK_Symbols_StoredImages_BitmapId" FOREIGN KEY ("BitmapId") REFERENCES public."StoredImages"("Id") ON DELETE RESTRICT;


--
-- Name: Templates FK_Templates_Drawings_DrawingId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Templates"
    ADD CONSTRAINT "FK_Templates_Drawings_DrawingId" FOREIGN KEY ("DrawingId") REFERENCES public."Drawings"("Id") ON DELETE RESTRICT;


--
-- Name: UserDrawings FK_UserDrawings_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."UserDrawings"
    ADD CONSTRAINT "FK_UserDrawings_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE RESTRICT;


--
-- Name: UserDrawings FK_UserDrawings_Drawings_DrawingId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."UserDrawings"
    ADD CONSTRAINT "FK_UserDrawings_Drawings_DrawingId" FOREIGN KEY ("DrawingId") REFERENCES public."Drawings"("Id") ON DELETE CASCADE;


--
-- Name: Views FK_Views_Drawings_DrawingId; Type: FK CONSTRAINT; Schema: public; Owner: openld
--

ALTER TABLE ONLY public."Views"
    ADD CONSTRAINT "FK_Views_Drawings_DrawingId" FOREIGN KEY ("DrawingId") REFERENCES public."Drawings"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

