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

--
-- Data for Name: AspNetRoles; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp") FROM stdin;
\.


--
-- Data for Name: AspNetRoleClaims; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."AspNetRoleClaims" ("Id", "RoleId", "ClaimType", "ClaimValue") FROM stdin;
\.


--
-- Data for Name: AspNetUsers; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."AspNetUsers" ("Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount") FROM stdin;
\.


--
-- Data for Name: AspNetUserClaims; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."AspNetUserClaims" ("Id", "UserId", "ClaimType", "ClaimValue") FROM stdin;
\.


--
-- Data for Name: AspNetUserLogins; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."AspNetUserLogins" ("LoginProvider", "ProviderKey", "ProviderDisplayName", "UserId") FROM stdin;
\.


--
-- Data for Name: AspNetUserRoles; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."AspNetUserRoles" ("UserId", "RoleId") FROM stdin;
\.


--
-- Data for Name: AspNetUserTokens; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."AspNetUserTokens" ("UserId", "LoginProvider", "Name", "Value") FROM stdin;
\.


--
-- Data for Name: DeviceCodes; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."DeviceCodes" ("UserCode", "DeviceCode", "SubjectId", "ClientId", "CreationTime", "Expiration", "Data") FROM stdin;
\.


--
-- Data for Name: Drawings; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."Drawings" ("Id", "Title", "OwnerId", "LastModified") FROM stdin;
aad634d7-1be9-4c1a-b4c1-126be62c57ae	WAC Studio	e0658ebd-6859-4ba7-81be-24a516493f90	2020-02-01 15:24:41.331909
5385bcec-ef1f-48eb-b9f8-8f00737e005e	WAC Theatre	e0658ebd-6859-4ba7-81be-24a516493f90	2020-02-01 15:28:23.048539
\.


--
-- Data for Name: FixtureTypes; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."FixtureTypes" ("Id", "Name") FROM stdin;
05b5b806-884f-41ad-9ff8-932f66d4d824	Profile (Conventional)
5b0c2683-5ac9-4bb3-a695-5482120176a8	Profile (LED)
2ed39877-bdf4-45df-a9fe-48d12a3054c3	Fresnel (Conventional)
83bae662-0375-421f-8e4a-b8fd6d421d87	Fresnel (LED)
5d1c85c0-9d0d-4559-a083-f08ef490beda	PAR (Conventional)
671a4f06-53ff-4da4-b9ba-ea898a94d7ac	PAR (LED)
94e63ef9-573c-4e4b-b2cd-10477d277daf	Flood (Conventional)
b0c7dc7b-bf8c-4195-ba37-ccf16dd541ea	Flood (LED)
5ff482ab-3ed3-4cd2-b380-a6db936bbf00	Intelligent
ee4fbca7-6564-4192-a1b2-e4c45d32789c	Strobe
9193519b-21be-4ca9-b99d-7f4929d05e5f	Smoke/Haze
a0fe4dc0-1651-45db-9069-dcd8bb7ccc12	Other
b98eb6d1-0e19-468d-8808-568185c05879	Moving (Beam)
bf10c60c-3452-4b4d-b14d-74d345b5daa1	Moving (Spot)
c3b593b7-e9ad-41ea-befd-7df4fe79e44b	Moving (Wash)
\.


--
-- Data for Name: StoredImages; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."StoredImages" ("Id", "Hash", "Path") FROM stdin;
166cfe0c-20e6-4196-8b9a-caa1225599ef	Hz7YEHXTfYmnbmAFdM6IGUFkKRtLJIEJpMa2pwzgZpk=	/openld-data/fixture-images/yfnmlx2c.mdi
d2f658d4-8345-4ac2-8c89-22c57e22bd47	1PkK13If1oZkuQxz20payCyHUSWQuAKEFUz+No2b7dc=	/openld-data/fixture-images/uv0i03oa.2e1
7d403d94-26be-400d-94b3-dfba56c0a3ee	F6Tv2F5B0oRpEaqIB/Xunvp5aDvFKa4GqFJMRqPQ9+A=	/openld-data/fixture-images/0zejqxpe.1eo
154e3be3-a9f9-4af1-b53f-d05062aeed41	K9/gVXC5AyLA09XL7X45K0VHzBznTd3PjvO0zBvlqiI=	/openld-data/fixture-images/rjv0h0iz.gyl
b26215d5-c6cf-455e-8207-f6f80765a546	VmmFibpDu6qKpnD1QM8Q8gR7ya5eXFvFxGrn5mZgSLU=	/openld-data/fixture-images/riwp11xf.jaj
bf18b825-56ac-4357-962d-c85143023fb3	9A50bKb0+aMxc2auSCcZe4hucGCpDJTg0Lr9iYrbZRg=	/openld-data/fixture-images/ddfrrwa4.jun
a62caaa6-04cc-49c6-9977-c3691d586226	xCObR6w8/bp5egjaB9EVoFbo/E+PW10kaungTCsUfjE=	/openld-data/fixture-images/g3mannmb.ea3
85fafbab-1833-4111-abb9-89cd3bff8c66	4KpkPUfwSTdXbfEI3vYKhhDw3hT20kcM4BY8RaMdVAo=	/openld-data/fixture-images/u3zdjaou.reh
8ace0083-9062-4326-9b6d-86c2592bdee9	OxGIjGWEdHhgsFWdNzNNZba1l3zyb6788k6xZ4AFeP0=	/openld-data/fixture-images/3kaq5ogc.ebs
07103a66-b370-468b-8314-7254c237a1b4	cr0qoG1Es1IWgvP1eH9fwh2MD+N/IFi0L+PqBUY9yFU=	/openld-data/fixture-images/xathhpaq.lih
f7680755-3885-42ea-9ed1-0665a7068382	vE1bqI6kHNVee/vRx3UrdZmd01HvBmOq6oIshZF00V8=	/openld-data/fixture-images/lstlxyln.yqg
09957726-5be8-46ee-9ec0-e9cf6ded28eb	5ZvgpXfzTegdp5/4SeMMK6lPJHVKUBxccMfdBAw9iNY=	/openld-data/fixture-images/2unhu2ai.knk
2181bef5-1fc9-4973-a5f2-d0a8de97a6bf	ulkEE3DZnvZHBrWvJY2hS61PPjQwcDBiktJR+JEJEoo=	/openld-data/fixture-images/ukh5gj4p.azq
d32e30e3-022c-4aaf-a5b0-d53ffe03d5db	oImmMY6mVEKgyn0VG5+qRlGodtuWlJWVnip1lOrxAuY=	/openld-data/fixture-images/cgighigi.jks
ae0abe90-364c-48b8-9520-97e44bbb5c44	tBf5G9VXBi3WY61KWWZ+QaLnrhoj9eDs91SkPbln3TE=	/openld-data/fixture-images/wpoarhn3.png
03e4a5db-4db8-4e9c-aa99-95af65c9b93c	pS9H5Qbpz9Obpew5Yjyik6JtDZPXyknUyEp5Dl2lsf0=	/openld-data/fixture-symbol-bitmaps/oqpxiyjz.rq4
3570c9d1-3650-4e69-bdf4-06244e431049	8N/e0G00y1MI9LLYCSZRIfdI3qjVapGFWYzKSATKTWY=	/openld-data/fixture-symbol-bitmaps/al2i4cwa.tcj
af499756-a81f-406f-b941-d73fce153647	N5IYQArD/Q8mKMexxweowXl4fYOb9cJ7mOr3j7dNUB4=	/openld-data/fixture-symbol-bitmaps/2wva1wzr.g3y
87b473cd-d696-4e17-b9d6-069a747a21fd	RvkpeFj6bWWos74iz9Qlalm5XjLfwcqlSD28JTjEbBs=	/openld-data/fixture-symbol-bitmaps/wddqp1es.n0b
7e5965ec-a775-45b9-acba-12ac1e2e02ab	Ysv3G8JhhpYu/THMkeya6kTszRkoq5sED+DKYSAegQE=	/openld-data/fixture-symbol-bitmaps/rnswfh1d.kq3
4a2c7af3-465d-44a0-94ce-b82520f7d0c3	m+cEE/+PrPmHZuXQr7q1bEDR9l4lH4FMC8hmR0Fn/cg=	/openld-data/fixture-symbol-bitmaps/qjdllgup.xb1
4e0ee58b-11c5-4f56-bb60-725483378390	yeLdENNQjkQdS+Toi7Z7o3viUfe0hua+V+aXoBmTesQ=	/openld-data/fixture-symbol-bitmaps/4qrmhsmy.ovb
4e909ee7-7bfc-441d-b370-6e881a2fb08c	dxZdQC53JPe0erAAheEdCXSMnlO2YIFDbqIElq0rbjg=	/openld-data/fixture-symbol-bitmaps/gsqgtsab.kou
c0084c78-a455-4735-b764-13db104deda4	FkVJRCxAaVq6I4KmPu3w9/LCp7ZiU0xEpiBNBjUtB3A=	/openld-data/fixture-symbol-bitmaps/2t5c1ptj.q2o
d235feff-356a-4afe-a653-f5635e923d5f	hce4D9k+elY5g6HvN7eZOq79c63m4FUwz8z7PJv4m5Y=	/openld-data/fixture-symbol-bitmaps/b13c0j5v.x3s
c6d7bd11-5d1c-4237-b53d-5b632a8ca64c	tkeL2B//bPtGvECB8eMg7G0JujKQp7bBlC0WyqT7ULA=	/openld-data/fixture-symbol-bitmaps/akimyxt2.yab
1d34a57c-a6ed-4780-9aa1-ededd55fda1d	3fKm4why3ajvuiBBb7cOknNAs5EnNan5H7KGCaRTgqA=	/openld-data/fixture-symbol-bitmaps/4r4hhxxw.kee
8153b470-1207-447a-8ff6-d2d34b092526	q7rniPmE5KCBcL0+Ac5hMiqscyKhQkrEMw6WOPKO7qw=	/openld-data/fixture-symbol-bitmaps/mqjb2jlo.4dd
c3b63a6e-9d68-4abf-9106-f3ca88534a51	6wYkvYm9FAp6k/9BR0D5Boh76IK43cpGBPty5fFNsrA=	/openld-data/fixture-symbol-bitmaps/l3f4a1ix.e0t
68a0253b-94a8-456a-a8bb-69732657876a	isLb8Z0RFVWFbpTJr5PXlXkbUciOcyY+JdSa12L2LSE=	/openld-data/fixture-symbol-bitmaps/dxuscapp.qjc
677cd50e-9e4e-4894-a7fb-fb744b9c2e51	/yxZ47Op5RHAkjG9D/woQQLDLmFtFIuf5cZ0VmzrYKQ=	/openld-data/fixture-symbol-bitmaps/qnl4e2vi.ebz
\.


--
-- Data for Name: Symbols; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."Symbols" ("Id", "Hash", "Path", "BitmapId") FROM stdin;
27f6d3c1-708a-4c3f-9699-bdb2a282416d	dNucbn7Dl3jFvyLvowgSKedCaeOXTqz3cG4SSiEaWhw=	/openld-data/fixture-symbols/jvvs1vnx.gw0	03e4a5db-4db8-4e9c-aa99-95af65c9b93c
da8e6904-4e68-4c43-9b29-f4a8ddefa4e7	OeGPLKsEFQR1y6y2YEXJmjoBx+LldPNRj6tixT66at0=	/openld-data/fixture-symbols/pe0vnlrs.32p	3570c9d1-3650-4e69-bdf4-06244e431049
1b33c23a-2b11-470e-8561-4b80985b1e38	XWOEXUWcNyCVCjt3I/pwaqMubawCHX3Buj/nB/Ytn3E=	/openld-data/fixture-symbols/ahk1ttag.43p	af499756-a81f-406f-b941-d73fce153647
6d2f5ffd-1d28-40ef-a694-9f6402162eec	b39/Ihq0c2rELi5PX2/cfkxLHH1UW9ntvscn8F3PEDs=	/openld-data/fixture-symbols/fgkwat43.pyt	87b473cd-d696-4e17-b9d6-069a747a21fd
7c24af69-7630-4008-a36d-9c17d03b352a	XHAVCiTjs1NjwTtAEDG1ukqeDRISrdjwDPIGENP+JO4=	/openld-data/fixture-symbols/trukqb3u.x5x	7e5965ec-a775-45b9-acba-12ac1e2e02ab
dabd20e9-cf3a-4411-85a6-91d7a4f4dfc2	fSCJIGAk2dO/L5JUXpqGiZ5W9cuz0/+aL1GMswd0NiA=	/openld-data/fixture-symbols/vqtppsvb.jor	4a2c7af3-465d-44a0-94ce-b82520f7d0c3
cbd19707-55f5-447d-8bd5-c18990515538	6p1iLPKVr2diT7ElInxxeCEGNMG96vIhM/8Cz1LwEuc=	/openld-data/fixture-symbols/iizvi3rq.1gk	4e0ee58b-11c5-4f56-bb60-725483378390
3e1af05a-d6ca-4a46-856e-718df41799ab	XVjiifMEvtxt9x6KQhbN1feiI2JAlol/eFny8JCno/k=	/openld-data/fixture-symbols/4bhr4nkb.aij	4e909ee7-7bfc-441d-b370-6e881a2fb08c
2a7c4b5b-e667-476f-b08c-be78e57adcb4	7IrG0+AYmfXjn4CinJe2RdvXEm8jksJApgkO6tJMGZQ=	/openld-data/fixture-symbols/sicqdply.jwi	c0084c78-a455-4735-b764-13db104deda4
5111ea12-04f1-4a7c-8e42-29e5c78844e1	T5sR5nnv9xAYJHf6kQIawQTcXHYn5VuQFP0BF1AqRFA=	/openld-data/fixture-symbols/2v0oen1e.bxy	d235feff-356a-4afe-a653-f5635e923d5f
21b983fa-9acf-4d94-b18e-1fe977d90ee1	YsfX6c4K4GqLAhCS+jGSCZCWAWdotQpeM6FD9SsQWUQ=	/openld-data/fixture-symbols/wtrlzt2k.ffm	c6d7bd11-5d1c-4237-b53d-5b632a8ca64c
b6abef11-d07d-4449-bc50-1eb5c3f5231d	e+TOzw7Q8X3zFH3y0c5+Ppj5mebakxBtzBeBUEs6ezQ=	/openld-data/fixture-symbols/kueigvsh.4n5	1d34a57c-a6ed-4780-9aa1-ededd55fda1d
0b9a3ff2-ae90-46af-a870-11c527f46199	/QLScvPYl3H042OfJk2XuOeYQgJ8nujYoZeOzRZato4=	/openld-data/fixture-symbols/f02bdzrg.sla	8153b470-1207-447a-8ff6-d2d34b092526
66b1d027-99ff-4502-b716-0398e4e71fc3	w3YrR6yWLpIdaaBJHFjjVZMpV4ZOsNWBQbKY/nQb2/0=	/openld-data/fixture-symbols/anht4jjr.ztr	c3b63a6e-9d68-4abf-9106-f3ca88534a51
14ede821-f10e-4bde-b2a4-785fe8edd2e8	wjxvPfiUhkY/ie+6b4b8d3TTHrJfRlPnqQ/RD9R5MLk=	/openld-data/fixture-symbols/o1txqq3l.3bt	68a0253b-94a8-456a-a8bb-69732657876a
24cbbbc2-779f-4b44-9833-107170119fa5	oPx3JqY6Mz3sNngHU1N1lsYWC85bJ7q+7jCUI+aJyL4=	/openld-data/fixture-symbols/edshgxmk.u4o	677cd50e-9e4e-4894-a7fb-fb744b9c2e51
\.


--
-- Data for Name: Fixtures; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."Fixtures" ("Id", "Name", "Manufacturer", "ReleaseDate", "TypeId", "Power", "Weight", "ImageId", "SymbolId") FROM stdin;
79e99993-b37b-4877-9d61-d9f80ee322cf	SL 15/32	Strand	2019-12-12 20:03:01.632	05b5b806-884f-41ad-9ff8-932f66d4d824	600	7	166cfe0c-20e6-4196-8b9a-caa1225599ef	27f6d3c1-708a-4c3f-9699-bdb2a282416d
cd629400-12f6-4437-95ee-0711c4abe4d1	Acclaim Axial 24-44°	Selecon	2019-12-12 20:24:35.561	05b5b806-884f-41ad-9ff8-932f66d4d824	600	5.6	7d403d94-26be-400d-94b3-dfba56c0a3ee	1b33c23a-2b11-470e-8561-4b80985b1e38
b9365b6e-1f71-48d0-b168-3c857154b391	310HF (J1K)	Robert Juliat	2019-12-12 20:26:30.289	2ed39877-bdf4-45df-a9fe-48d12a3054c3	1000	9.2	154e3be3-a9f9-4af1-b53f-d05062aeed41	6d2f5ffd-1d28-40ef-a694-9f6402162eec
bc8ca6ca-6ff3-4058-a71e-5bb7a5a75547	MMX Spot	Robe	2019-12-12 20:45:25.192	bf10c60c-3452-4b4d-b14d-74d345b5daa1	1020	25.5	f7680755-3885-42ea-9ed1-0665a7068382	3e1af05a-d6ca-4a46-856e-718df41799ab
8370ccb2-e7a3-4be9-9df0-a0ff212612e2	329HPC (J2K)	Robert Juliat	2019-12-12 20:27:51.507	2ed39877-bdf4-45df-a9fe-48d12a3054c3	2000	16	b26215d5-c6cf-455e-8207-f6f80765a546	2a7c4b5b-e667-476f-b08c-be78e57adcb4
dd004aa1-dc24-47fe-9f55-c06f07884803	SL 23/50	Strand	2019-12-12 20:06:07.288	05b5b806-884f-41ad-9ff8-932f66d4d824	600	7	166cfe0c-20e6-4196-8b9a-caa1225599ef	da8e6904-4e68-4c43-9b29-f4a8ddefa4e7
9efe102a-1912-4189-8e7b-3b62a3070714	PAR 64	Generic	2019-12-12 20:29:55.195	5d1c85c0-9d0d-4559-a083-f08ef490beda	1000	1	bf18b825-56ac-4357-962d-c85143023fb3	7c24af69-7630-4008-a36d-9c17d03b352a
e61583db-2a4e-40b4-8de9-4a988ad3487b	STUDIOCOB FC	ProLights	2019-12-12 20:39:05.966	671a4f06-53ff-4da4-b9ba-ea898a94d7ac	150	4.5	85fafbab-1833-4111-abb9-89cd3bff8c66	dabd20e9-cf3a-4411-85a6-91d7a4f4dfc2
fa02dd06-a2a1-4be9-9cc1-4f980d9be9e5	RGB Power Batten	EQUINOX	2019-12-13 12:11:04.87	5ff482ab-3ed3-4cd2-b380-a6db936bbf00	30	1.8	d32e30e3-022c-4aaf-a5b0-d53ffe03d5db	cbd19707-55f5-447d-8bd5-c18990515538
92c28b92-5b52-4d75-8c19-ee0f941bbc2c	LUMIPAR (12HPRO)	ProLights	2020-01-18 13:20:11.018	671a4f06-53ff-4da4-b9ba-ea898a94d7ac	96	4.5	a62caaa6-04cc-49c6-9977-c3691d586226	66b1d027-99ff-4502-b716-0398e4e71fc3
97409f26-221a-45b4-87d2-df121ad8af2f	Shakespeare Zoom 15-35°	Altman	2019-12-12 20:22:54.624	05b5b806-884f-41ad-9ff8-932f66d4d824	600	9.9	d2f658d4-8345-4ac2-8c89-22c57e22bd47	14ede821-f10e-4bde-b2a4-785fe8edd2e8
94737e05-2449-489d-a0a3-3b6781d3f03d	REFLEX	ProLights	2019-12-12 20:41:04.525	c3b593b7-e9ad-41ea-befd-7df4fe79e44b	245	9.2	8ace0083-9062-4326-9b6d-86c2592bdee9	5111ea12-04f1-4a7c-8e42-29e5c78844e1
e748832b-eab3-4915-be84-88f8d05ecb42	LEDWash 800	Robe	2019-12-12 20:42:45.035	c3b593b7-e9ad-41ea-befd-7df4fe79e44b	430	10.9	07103a66-b370-468b-8314-7254c237a1b4	21b983fa-9acf-4d94-b18e-1fe977d90ee1
8e0eb758-59e5-4e91-a8d5-3a814506d827	DF-50 Diffusion Hazer	Reel EFX	2020-01-17 16:18:53.464	9193519b-21be-4ca9-b99d-7f4929d05e5f	400	16	09957726-5be8-46ee-9ec0-e9cf6ded28eb	b6abef11-d07d-4449-bc50-1eb5c3f5231d
cb322e2a-a92c-4fee-9bfa-2d805397b75d	MVS Hazer	Le Maitre	2020-01-17 17:03:48.236	9193519b-21be-4ca9-b99d-7f4929d05e5f	300	12	ae0abe90-364c-48b8-9520-97e44bbb5c44	0b9a3ff2-ae90-46af-a870-11c527f46199
fe9367ae-ab57-441f-8192-c6f65c7e9363	Ghibli	Ayrton	2020-01-17 16:28:44.583	bf10c60c-3452-4b4d-b14d-74d345b5daa1	800	35.6	2181bef5-1fc9-4973-a5f2-d0a8de97a6bf	24cbbbc2-779f-4b44-9833-107170119fa5
\.


--
-- Data for Name: FixtureModes; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."FixtureModes" ("Id", "Name", "FixtureId", "Channels") FROM stdin;
6d3293b3-d227-4214-ac0a-1060624569a0	Default	79e99993-b37b-4877-9d61-d9f80ee322cf	1
bb584324-2184-4559-b837-80fafa6168a0	Default	cd629400-12f6-4437-95ee-0711c4abe4d1	1
d5675bc3-4407-43f1-a2b1-45e8ac9a3d7d	Default	b9365b6e-1f71-48d0-b168-3c857154b391	1
5b3b33d1-b9b8-46dc-bdde-661209d556ab	Default	8370ccb2-e7a3-4be9-9df0-a0ff212612e2	1
76a10664-8c96-49ed-87c5-65edc4bbbe10	Default	dd004aa1-dc24-47fe-9f55-c06f07884803	1
9d5331ee-a4a9-4c8f-9bcc-6b9469915114	Default	9efe102a-1912-4189-8e7b-3b62a3070714	1
ce06bc1a-f9e4-45de-a04f-b5768142496e	Default	97409f26-221a-45b4-87d2-df121ad8af2f	1
be3980af-8037-45c7-80a2-8b0b57d3d351	Default	8e0eb758-59e5-4e91-a8d5-3a814506d827	1
5077f399-fd42-4215-943d-aff4c98f3497	Default	cb322e2a-a92c-4fee-9bfa-2d805397b75d	4
125c112e-839a-43ee-9245-d5e371640706	1	bc8ca6ca-6ff3-4058-a71e-5bb7a5a75547	38
45574210-313a-4345-96ef-e370a4fddd0a	2	bc8ca6ca-6ff3-4058-a71e-5bb7a5a75547	31
c1c682d0-d701-4de5-ba40-111181f48dd7	3	bc8ca6ca-6ff3-4058-a71e-5bb7a5a75547	29
ca119886-5d71-4558-9d47-d785860eda73	4	bc8ca6ca-6ff3-4058-a71e-5bb7a5a75547	40
149d6de5-921c-4e0c-afec-41ff24790f4e	3 Channel (RGB)	e61583db-2a4e-40b4-8de9-4a988ad3487b	3
b5d61284-2050-4c68-b9a5-dbc1b835d893	5 Channel (Dimmer, RGB, Strobe)	e61583db-2a4e-40b4-8de9-4a988ad3487b	5
5b6dbb4e-3c3c-4ad9-8402-9be77b783d55	8 Channel (Dimmer, RGB, Strobe/Speed, Color Macros, Mode, Dimmer Speed)	e61583db-2a4e-40b4-8de9-4a988ad3487b	8
e9b2b497-e52c-4bf2-a24e-5d2ec33c6a0d	P1	fa02dd06-a2a1-4be9-9cc1-4f980d9be9e5	3
b669601f-d5ae-429c-b551-278a87e782db	P2	fa02dd06-a2a1-4be9-9cc1-4f980d9be9e5	4
7fac74af-032a-454c-9e82-ce5c9deabbd8	P3	fa02dd06-a2a1-4be9-9cc1-4f980d9be9e5	14
87dfea56-55c5-4a37-94f6-a7374a0291ca	P4	fa02dd06-a2a1-4be9-9cc1-4f980d9be9e5	26
105b66bc-3e44-42a2-8b7f-f8be179899e8	P5	fa02dd06-a2a1-4be9-9cc1-4f980d9be9e5	2
8160edda-4d88-4edb-880f-08992b1728eb	P6	fa02dd06-a2a1-4be9-9cc1-4f980d9be9e5	7
c2960074-ceb9-4945-9769-18462e6914f4	3 Channel (HSV)	92c28b92-5b52-4d75-8c19-ee0f941bbc2c	3
03c040fa-0551-4a53-9432-738d288cbbd1	3 Channel (HSI)	92c28b92-5b52-4d75-8c19-ee0f941bbc2c	3
115420ec-d5f8-4328-b7c4-54d05454dab2	4 Channel (RGBW)	92c28b92-5b52-4d75-8c19-ee0f941bbc2c	4
48ed5c83-a28e-4f7a-a164-9a3ce45d4e12	6 Channel (RGBWAP)	92c28b92-5b52-4d75-8c19-ee0f941bbc2c	6
89f4c658-d2cd-4628-bee8-579c4c65fcbe	8 Channel (Dimmer, RGBAWP, Strobe)	92c28b92-5b52-4d75-8c19-ee0f941bbc2c	8
dc598c86-83d1-4ce0-b73f-bf9f73c0c59b	12 Channel (Dimmer, RGBAWP, Strobe, Macro, Auto, Auto Speed, Dimmer Curve)	92c28b92-5b52-4d75-8c19-ee0f941bbc2c	12
c9efbe27-8257-4330-a27d-b2cd4e81c82f	Default	94737e05-2449-489d-a0a3-3b6781d3f03d	15
6c6f12c9-a810-40ca-8b99-79e518795f07	1	e748832b-eab3-4915-be84-88f8d05ecb42	38
d9a68761-8a5d-49d9-9109-b7241cc9e5ed	2	e748832b-eab3-4915-be84-88f8d05ecb42	22
649036a5-f525-44b7-b3fa-81beedf4810b	3	e748832b-eab3-4915-be84-88f8d05ecb42	16
221ac6c0-0536-4d81-83e2-59b9fb32ea00	4	e748832b-eab3-4915-be84-88f8d05ecb42	11
f010389e-51f3-4660-9aff-c9965c16bcca	5	e748832b-eab3-4915-be84-88f8d05ecb42	38
f4c1e191-646f-4716-92a9-e18059325c76	6	e748832b-eab3-4915-be84-88f8d05ecb42	37
da216e40-2bfe-4bc6-a5e7-850cc772494f	Standard	fe9367ae-ab57-441f-8192-c6f65c7e9363	38
dfa996b6-873d-48cd-99e1-8f6752fd325e	Basic	fe9367ae-ab57-441f-8192-c6f65c7e9363	36
afcc5da0-78e2-44a7-81f8-409bb29abcf1	Extended	fe9367ae-ab57-441f-8192-c6f65c7e9363	58
\.


--
-- Data for Name: PersistedGrants; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."PersistedGrants" ("Key", "Type", "SubjectId", "ClientId", "CreationTime", "Expiration", "Data") FROM stdin;
\.


--
-- Data for Name: StructureTypes; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."StructureTypes" ("Id", "Name") FROM stdin;
2d47b935-9e5c-468d-9f51-1f751827906b	Internally Wired Bar
7ffb32a3-5081-4207-9955-394d0fdba5d9	Winch Bar
03b268a0-c30e-4e8a-82bc-b51d383d4ce5	Fly Bar
5204943d-57be-42fe-86b5-486a06183381	Deck
16c282f2-2560-4c1d-805d-6a61c3f0ddb6	Truss
36a1c93a-8874-405e-8908-6ef2cae707b1	Ladder Truss
24976a58-9ccc-4492-aaf2-7f5e2c4471b0	Stand
fb2d4bf3-937e-42d1-9444-dcc045b952d1	Floor
36cde518-4455-4d17-93c7-7fdc019a8bc0	Other
20063996-b8b1-4b09-983f-619e19b439f2	Bar
\.


--
-- Data for Name: Views; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."Views" ("Id", "DrawingId", "Name", "Type", "Height", "Width") FROM stdin;
1fb479f7-4528-41d7-aed0-bc47e14d1e17	aad634d7-1be9-4c1a-b4c1-126be62c57ae	Grid	0	15	15
34597b7f-e190-48d9-a880-a7399466ade6	aad634d7-1be9-4c1a-b4c1-126be62c57ae	Tech Balcony	0	15	15
8caadd25-4b5a-44d2-beac-df8acb4c6a6f	aad634d7-1be9-4c1a-b4c1-126be62c57ae	Audience Balcony	0	15	15
4f16eedd-ab17-45c8-8bd6-c31f58b0abf1	aad634d7-1be9-4c1a-b4c1-126be62c57ae	Floor	0	15	15
a2e9fd60-ba76-4906-93b0-3ba61435326b	5385bcec-ef1f-48eb-b9f8-8f00737e005e	Theatre	0	20	20
\.


--
-- Data for Name: Structures; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."Structures" ("Id", "ViewId", "Geometry", "Name", "Rating", "TypeId", "Notes") FROM stdin;
58ee3621-c180-4d9d-807f-7edd28cafaf1	1fb479f7-4528-41d7-aed0-bc47e14d1e17	{"Points": [{"x": 0.2, "y": 0.2}, {"x": 0.2, "y": 14.8}, {"x": 14.8, "y": 14.8}, {"x": 14.8, "y": 0.2}, {"x": 0.2, "y": 0.2}]}	\N	0	20063996-b8b1-4b09-983f-619e19b439f2
e13049c3-1e6b-4704-9a50-3a613a6f4046	34597b7f-e190-48d9-a880-a7399466ade6	{"Points": [{"x": 1.2000000000000002, "y": 1.4000000000000001}, {"x": 1.2000000000000002, "y": 13.8}, {"x": 13.8, "y": 13.8}, {"x": 13.8, "y": 1.4000000000000001}, {"x": 1.2000000000000002, "y": 1.4000000000000001}]}	\N	0	20063996-b8b1-4b09-983f-619e19b439f2
c2fa92fb-07bf-4ee7-9217-8ed7f83ab432	8caadd25-4b5a-44d2-beac-df8acb4c6a6f	{"Points": [{"x": 0.2, "y": 0.2}, {"x": 0.2, "y": 14.8}, {"x": 14.8, "y": 14.8}, {"x": 14.8, "y": 0.2}, {"x": 0.2, "y": 0.2}]}	\N	0	20063996-b8b1-4b09-983f-619e19b439f2
e2270e1d-7ea4-4eff-8077-4e0ef2c02f0e	8caadd25-4b5a-44d2-beac-df8acb4c6a6f	{"Points": [{"x": 1.2000000000000002, "y": 1.4000000000000001}, {"x": 1.2000000000000002, "y": 13.8}, {"x": 13.8, "y": 13.8}, {"x": 13.8, "y": 1.4000000000000001}, {"x": 1.2000000000000002, "y": 1.4000000000000001}]}	\N	0	20063996-b8b1-4b09-983f-619e19b439f2
1ee6bde1-7d93-48f7-ad04-e1729a1e1957	1fb479f7-4528-41d7-aed0-bc47e14d1e17	{"Points": [{"x": 4.7, "y": 1.4000000000000001}, {"x": 4.7, "y": 13.8}, {"x": 7.2, "y": 13.8}, {"x": 7.2, "y": 1.4000000000000001}, {"x": 4.7, "y": 1.4000000000000001}]}		0	20063996-b8b1-4b09-983f-619e19b439f2
39e15b81-357c-40d9-a3af-1c0606dc9dd2	1fb479f7-4528-41d7-aed0-bc47e14d1e17	{"Points": [{"x": 3.5, "y": 13.600000000000001}, {"x": 3.5, "y": 1.6}]}	W7	0	20063996-b8b1-4b09-983f-619e19b439f2
af5cc476-ee50-416d-812a-a217ecf61a6b	34597b7f-e190-48d9-a880-a7399466ade6	{"Points": [{"x": 0.2, "y": 0.2}, {"x": 0.2, "y": 14.8}, {"x": 14.8, "y": 14.8}, {"x": 14.8, "y": 0.2}, {"x": 0.2, "y": 0.2}]}	\N	0	20063996-b8b1-4b09-983f-619e19b439f2
c8b7396d-98ee-419d-b9dd-b37da10baf97	1fb479f7-4528-41d7-aed0-bc47e14d1e17	{"Points": [{"x": 8.0, "y": 1.4000000000000001}, {"x": 8.0, "y": 13.8}, {"x": 10.5, "y": 13.8}, {"x": 10.5, "y": 1.4000000000000001}, {"x": 8.0, "y": 1.4000000000000001}]}		0	20063996-b8b1-4b09-983f-619e19b439f2
cf8fd44d-4b3a-481a-970d-eac850ab5b5f	1fb479f7-4528-41d7-aed0-bc47e14d1e17	{"Points": [{"x": 11.3, "y": 1.4000000000000001}, {"x": 11.3, "y": 13.8}, {"x": 13.8, "y": 13.8}, {"x": 13.8, "y": 1.4000000000000001}, {"x": 11.3, "y": 1.4000000000000001}]}	\N	0	20063996-b8b1-4b09-983f-619e19b439f2
b2f41a75-f467-4f41-bc78-45a0126cba40	1fb479f7-4528-41d7-aed0-bc47e14d1e17	{"Points": [{"x": 10.100000000000001, "y": 13.600000000000001}, {"x": 10.100000000000001, "y": 1.6}]}	W3	0	20063996-b8b1-4b09-983f-619e19b439f2
da565d02-7a4a-403f-b3d7-8d60a1d8762e	1fb479f7-4528-41d7-aed0-bc47e14d1e17	{"Points": [{"x": 1.4, "y": 1.4000000000000001}, {"x": 1.4, "y": 13.8}, {"x": 3.9000000000000004, "y": 13.8}, {"x": 3.9000000000000004, "y": 1.4000000000000001}, {"x": 1.4, "y": 1.4000000000000001}]}		0	20063996-b8b1-4b09-983f-619e19b439f2
05d92ee5-2a44-4143-84d1-10c8d530b590	1fb479f7-4528-41d7-aed0-bc47e14d1e17	{"Points": [{"x": 1.8, "y": 13.600000000000001}, {"x": 1.8, "y": 1.6}]}	W8	0	20063996-b8b1-4b09-983f-619e19b439f2
e4b2c771-d9c4-4b5a-8f83-3689bc5ecfb4	1fb479f7-4528-41d7-aed0-bc47e14d1e17	{"Points": [{"x": 5.1000000000000005, "y": 13.600000000000001}, {"x": 5.1000000000000005, "y": 1.6}]}	W6	0	20063996-b8b1-4b09-983f-619e19b439f2
95feb4de-0cfc-4d5d-bd88-811dbb6433a1	1fb479f7-4528-41d7-aed0-bc47e14d1e17	{"Points": [{"x": 6.7, "y": 13.600000000000001}, {"x": 6.7, "y": 1.6}]}	W5	0	20063996-b8b1-4b09-983f-619e19b439f2
53593bc2-793a-46f1-96d9-0e5063759eaa	1fb479f7-4528-41d7-aed0-bc47e14d1e17	{"Points": [{"x": 11.700000000000001, "y": 13.600000000000001}, {"x": 11.700000000000001, "y": 1.6}]}	W2	0	20063996-b8b1-4b09-983f-619e19b439f2
48a67617-580a-460d-b25b-9339f31b2d83	1fb479f7-4528-41d7-aed0-bc47e14d1e17	{"Points": [{"x": 13.4, "y": 13.600000000000001}, {"x": 13.4, "y": 1.6}]}	W1	0	20063996-b8b1-4b09-983f-619e19b439f2
66be999d-ca0e-4f99-a26c-313df6c91bcc	1fb479f7-4528-41d7-aed0-bc47e14d1e17	{"Points": [{"x": 8.4, "y": 13.600000000000001}, {"x": 8.4, "y": 1.6}]}	W4	0	20063996-b8b1-4b09-983f-619e19b439f2	Damaged, cannot be winched
04cd5985-8d19-4ba2-b975-7730412f30a0	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 0.19999999999999996, "y": 5.5}, {"x": 4.800000000000001, "y": 0.9000000000000004}, {"x": 15.200000000000001, "y": 0.9000000000000004}, {"x": 19.8, "y": 5.5}]}	Bridge 3	0	20063996-b8b1-4b09-983f-619e19b439f2
b718d1e6-5a7a-4c92-a67d-3f6ffc850714	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 3.1, "y": 8.4}, {"x": 6.5, "y": 5.0}, {"x": 13.4, "y": 5.0}, {"x": 16.900000000000002, "y": 8.4}]}	Bridge 2	0	20063996-b8b1-4b09-983f-619e19b439f2
5ec23f18-92d4-4e9d-a60c-2ec4728d5fed	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 4.6000000000000005, "y": 9.8}, {"x": 7.4, "y": 7.1000000000000005}, {"x": 12.600000000000001, "y": 7.1000000000000005}, {"x": 15.200000000000001, "y": 9.8}, {"x": 4.6000000000000005, "y": 9.8}]}	Bridge 1	0	20063996-b8b1-4b09-983f-619e19b439f2
7b26c8af-f6d5-45e4-9bd5-e69a28294ec9	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 0.2, "y": 14.8}, {"x": 4.5, "y": 19.1}, {"x": 15.5, "y": 19.1}, {"x": 19.8, "y": 14.8}]}	Wall	0	36cde518-4455-4d17-93c7-7fdc019a8bc0
e378f96b-5730-417a-8c42-937d5b5f228f	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 15.5, "y": 11.9}, {"x": 16.5, "y": 11.9}]}	6	0	20063996-b8b1-4b09-983f-619e19b439f2
b0ec1a88-081b-4e88-8f13-dc86754e4aa8	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 15.700000000000001, "y": 12.100000000000001}, {"x": 16.7, "y": 12.100000000000001}]}	7	0	20063996-b8b1-4b09-983f-619e19b439f2
7775aa34-87b1-4255-be3b-26fbf647cc14	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 14.8, "y": 12.4}, {"x": 19.8, "y": 6.9}]}	Tormentors	0	20063996-b8b1-4b09-983f-619e19b439f2
116001b7-5784-4a76-b4f8-7044921ce0f8	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 15.9, "y": 12.3}, {"x": 16.900000000000002, "y": 12.3}]}	8	0	20063996-b8b1-4b09-983f-619e19b439f2
0f822d56-bf7f-46b3-a788-c6b9d61e8222	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 0.2, "y": 6.9}, {"x": 5.2, "y": 12.4}]}	Tormentors	0	20063996-b8b1-4b09-983f-619e19b439f2
ae2c1749-a210-4218-8ff4-f84b50864198	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 14.600000000000001, "y": 10.9}, {"x": 15.600000000000001, "y": 10.9}]}	2	0	03b268a0-c30e-4e8a-82bc-b51d383d4ce5
3045ef79-efd6-4bcd-a615-ea2b3c970702	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 16.1, "y": 12.5}, {"x": 17.1, "y": 12.5}]}	9	0	20063996-b8b1-4b09-983f-619e19b439f2
cee73c91-2ef1-47b2-98f2-e173c76bcb0a	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 14.8, "y": 11.200000000000001}, {"x": 15.8, "y": 11.200000000000001}]}	3	0	20063996-b8b1-4b09-983f-619e19b439f2
1d799cbc-1223-4c8c-b622-ce832405292e	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 15.100000000000001, "y": 11.4}, {"x": 16.1, "y": 11.4}]}	4	0	20063996-b8b1-4b09-983f-619e19b439f2
7132f45e-084e-4442-a86b-50b5a2e6d41e	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 15.3, "y": 11.700000000000001}, {"x": 16.3, "y": 11.700000000000001}]}	5	0	20063996-b8b1-4b09-983f-619e19b439f2
c378b207-a47c-4a24-8adb-0d42ea163a42	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 17.400000000000002, "y": 13.8}, {"x": 18.400000000000002, "y": 13.8}]}	14	0	20063996-b8b1-4b09-983f-619e19b439f2
c7b9512c-2ca7-4574-9b08-60a3b65e41f4	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 16.900000000000002, "y": 13.3}, {"x": 17.900000000000002, "y": 13.3}]}	12	0	20063996-b8b1-4b09-983f-619e19b439f2
f9ec4d00-79cd-4ef9-be24-37cbe15f33d3	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 17.900000000000002, "y": 14.4}, {"x": 18.900000000000002, "y": 14.4}]}	17	0	20063996-b8b1-4b09-983f-619e19b439f2
9c0877a2-d97a-4e4b-8447-6288dbbf9ce2	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 16.400000000000002, "y": 12.700000000000001}, {"x": 17.400000000000002, "y": 12.700000000000001}]}	10	0	20063996-b8b1-4b09-983f-619e19b439f2
4d7c0a10-fc10-404c-b63a-1e2f5a675e73	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 16.6, "y": 13.0}, {"x": 17.6, "y": 13.0}]}	11	0	20063996-b8b1-4b09-983f-619e19b439f2
ba1a2f7b-bd48-476b-9583-a52cc29322cc	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 17.8, "y": 14.0}, {"x": 18.8, "y": 14.0}]}	15	0	20063996-b8b1-4b09-983f-619e19b439f2
f215e245-b572-426b-bd2c-9a33beeb45ec	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 17.2, "y": 13.600000000000001}, {"x": 18.2, "y": 13.600000000000001}]}	13	0	20063996-b8b1-4b09-983f-619e19b439f2
bc42d319-e6f4-4256-ad25-3c03e48d36ad	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 18.1, "y": 14.200000000000001}, {"x": 19.1, "y": 14.200000000000001}]}	16	0	20063996-b8b1-4b09-983f-619e19b439f2
dba049a8-2f29-42f3-af1c-9484878322f4	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 17.8, "y": 14.600000000000001}, {"x": 18.8, "y": 14.600000000000001}]}	18	0	20063996-b8b1-4b09-983f-619e19b439f2
4b30ce80-71c3-4944-8079-12c4db4e452a	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 1.5, "y": 14.8}, {"x": 2.5, "y": 14.8}]}	19	0	20063996-b8b1-4b09-983f-619e19b439f2
1a4304f6-8003-4ce8-892e-33d0ff823dc6	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 1.7000000000000002, "y": 15.0}, {"x": 2.7, "y": 15.0}]}	20	0	20063996-b8b1-4b09-983f-619e19b439f2
8c7fbb81-fd98-4999-a113-ebb412fe77cd	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 1.9000000000000001, "y": 15.3}, {"x": 2.9000000000000004, "y": 15.3}]}	21	0	20063996-b8b1-4b09-983f-619e19b439f2
8df25bcc-1057-4849-bd4f-cd935f49ef58	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 2.1, "y": 15.600000000000001}, {"x": 3.1, "y": 15.600000000000001}]}	22	0	20063996-b8b1-4b09-983f-619e19b439f2
8a23cf38-2696-4ef9-a587-4c04b2ff338e	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 2.4000000000000004, "y": 15.8}, {"x": 3.4000000000000004, "y": 15.8}]}	23	0	20063996-b8b1-4b09-983f-619e19b439f2
52f93e6e-a9bb-4848-8457-fc06fb3f9d7d	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 2.6, "y": 16.1}, {"x": 3.6, "y": 16.1}]}	24	0	20063996-b8b1-4b09-983f-619e19b439f2
1a3dade2-f974-43fd-8d10-08261e725e61	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 0.6000000000000001, "y": 14.8}, {"x": 4.6000000000000005, "y": 18.8}, {"x": 15.4, "y": 18.8}, {"x": 19.400000000000002, "y": 14.8}]}	36 (Wrap Around)	0	20063996-b8b1-4b09-983f-619e19b439f2
04f2a7de-cb79-460c-bc92-54fbded7a5f4	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 2.8000000000000003, "y": 16.3}, {"x": 3.8000000000000003, "y": 16.3}]}	25	0	20063996-b8b1-4b09-983f-619e19b439f2
ca6d1cd4-3238-4e85-a573-99f08495697c	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 3.0, "y": 16.6}, {"x": 4.0, "y": 16.6}]}	26	0	20063996-b8b1-4b09-983f-619e19b439f2
0c1990c2-f62d-4eac-8955-a2d9014d1d79	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 3.3000000000000003, "y": 16.8}, {"x": 4.3, "y": 16.8}]}	27	0	20063996-b8b1-4b09-983f-619e19b439f2
91f0e44b-e571-4cda-a200-aef20f6e7887	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 3.5, "y": 17.1}, {"x": 4.5, "y": 17.1}]}	28	0	20063996-b8b1-4b09-983f-619e19b439f2
2e05df6b-5262-409b-bfb1-dec19b5fd259	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 3.7, "y": 17.3}, {"x": 4.7, "y": 17.3}]}	29	0	20063996-b8b1-4b09-983f-619e19b439f2
75faa1ae-49fd-4537-8b13-05cc1737ae09	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 4.0, "y": 17.6}, {"x": 5.0, "y": 17.6}]}	30	0	20063996-b8b1-4b09-983f-619e19b439f2
8668b3ea-a4fe-44d9-a646-a609fabf2bf3	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 4.2, "y": 17.8}, {"x": 5.2, "y": 17.8}]}	31	0	20063996-b8b1-4b09-983f-619e19b439f2
224f6671-b8ce-4988-a975-24aa0b7dcaef	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 4.4, "y": 18.1}, {"x": 5.4, "y": 18.1}]}	32	0	20063996-b8b1-4b09-983f-619e19b439f2
e2a9501f-30d0-4982-b5c7-f2c2552cb030	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 4.800000000000001, "y": 18.400000000000002}, {"x": 5.800000000000001, "y": 18.400000000000002}]}	33	0	20063996-b8b1-4b09-983f-619e19b439f2
ebe71eca-f5b5-4eee-ad81-e7a5003c0780	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 15.4, "y": 18.400000000000002}, {"x": 19.200000000000003, "y": 14.600000000000001}]}	35 (Side Bar)	0	20063996-b8b1-4b09-983f-619e19b439f2
42d64f9c-486c-46b8-a899-d666f2424ef4	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 6.699999999999999, "y": 5.300000000000001}, {"x": 13.200000000000003, "y": 5.300000000000001}]}	\N	0	7ffb32a3-5081-4207-9955-394d0fdba5d9
5f0ef14f-d16b-4d55-80af-3951ad34751f	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 0.8, "y": 14.600000000000001}, {"x": 4.7, "y": 18.5}]}	34 (Side Bar)	0	20063996-b8b1-4b09-983f-619e19b439f2
4c662c3d-6e4e-4367-84b1-819c47e16738	a2e9fd60-ba76-4906-93b0-3ba61435326b	{"Points": [{"x": 5.0, "y": 1.2000000000000002}, {"x": 15.0, "y": 1.2000000000000002}]}	\N	0	7ffb32a3-5081-4207-9955-394d0fdba5d9
72da08ea-ca1c-4775-bbb8-de21a2ef0556	4f16eedd-ab17-45c8-8bd6-c31f58b0abf1	{"Points": [{"x": 0.2, "y": 0.2}, {"x": 0.2, "y": 14.8}, {"x": 14.8, "y": 14.8}, {"x": 14.8, "y": 0.2}, {"x": 0.2, "y": 0.2}]}	\N	0	20063996-b8b1-4b09-983f-619e19b439f2
\.


--
-- Data for Name: RiggedFixtures; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."RiggedFixtures" ("Id", "FixtureId", "StructureId", "Position", "Address", "Universe", "ModeId", "Notes", "Angle", "Colour", "Name", "Channel") FROM stdin;
\.


--
-- Data for Name: Templates; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."Templates" ("Id", "DrawingId") FROM stdin;
c8fbe928-90c9-4a4e-9328-78bde62c22f8	aad634d7-1be9-4c1a-b4c1-126be62c57ae
e0fc4abf-7a74-4e63-b2f0-623a6f20c361	5385bcec-ef1f-48eb-b9f8-8f00737e005e
\.


--
-- Data for Name: UserDrawings; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."UserDrawings" ("Id", "UserId", "DrawingId") FROM stdin;
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: openld
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20191228112834_RemovePostGIS	3.1.0
20200102132022_DrawingSize	3.1.0
20200106093404_ViewSize	3.1.0
20200106112912_CascadeDelete	3.1.0
20200117155630_FixtureModes	3.1.0
20200117160041_FixtureModeChannels	3.1.0
20200117161619_CascadeModesUserDrawing	3.1.0
20200122153221_RiggedFixture_Name-Colour-Angle	3.1.0
20200125164625_FixtureSymbolBitmaps	3.1.0
20200125200025_DrawingTemplates	3.1.0
20200130095115_RiggedFixtureChannel	3.1.0
\.


--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: openld
--

SELECT pg_catalog.setval('public."AspNetRoleClaims_Id_seq"', 1, false);


--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: openld
--

SELECT pg_catalog.setval('public."AspNetUserClaims_Id_seq"', 1, false);


--
-- PostgreSQL database dump complete
--

