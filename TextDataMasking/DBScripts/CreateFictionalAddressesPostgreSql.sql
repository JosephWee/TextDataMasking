CREATE DATABASE "DataMaskingDB";

\c "DataMaskingDB"

CREATE TABLE "public"."Address"
(
	"AddressID"    integer     PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
	"AddressLine1" varchar(60) NOT NULL,
	"AddressLine2" varchar(60) NULL,
	"CityID"       integer     NULL,
	"CountryID"    integer     NOT NULL,
	"PostalCode"   varchar(15) NOT NULL,
	"MapDataJson"  JSON,
    "MapDataXml"   XML,
	"ModifiedDate" timestamptz NOT NULL DEFAULT (current_timestamp at time zone 'UTC')
);

\copy "public"."Address" FROM './TextDataMasking/DBScripts/Address2023_06_12_1102.csv' WITH CSV HEADER DELIMITER ','

DO $$
    DECLARE
       q1 integer; 
    BEGIN

    SELECT 1 INTO q1 FROM pg_catalog.pg_roles WHERE rolname = 'DataMaskingUser';

    IF NOT FOUND THEN
        CREATE USER "DataMaskingUser" WITH PASSWORD 'DataMaskingPassword';
    END IF;
   
END; $$;

GRANT SELECT, INSERT, UPDATE, DELETE
ON ALL TABLES IN SCHEMA PUBLIC
TO "DataMaskingUser";

\quit