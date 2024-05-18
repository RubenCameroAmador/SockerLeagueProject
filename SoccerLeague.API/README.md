
#Posgres


- docker run -d -i --name postgres -p 5432:5432 -e POSTGRES_HOST_AUTH_METHOD=trust postgres
- docker run --name postgres -e POSTGRES_PASSWORD=admin1234 -d postgres

- init psql bash mode
psql -h localhost -U postgres

- to connect a db <postgres>
\c postgres

- to list tables of db
\dt


--TABLES OF APPLICATION SOCCER LEAGUE

-- Table: public.team

-- DROP TABLE IF EXISTS public.team;
CREATE SEQUENCE team_id_seq START 1 INCREMENT BY 1;
CREATE TABLE IF NOT EXISTS public.team
(
    id integer NOT NULL DEFAULT nextval('team_id_seq'::regclass),
    name character varying COLLATE pg_catalog."default",
    date_created timestamp without time zone,
    last_update timestamp without time zone,
    CONSTRAINT team_pkey PRIMARY KEY (id)
);

-- Table: public.teams_matches

-- DROP TABLE IF EXISTS public.teams_matches;
CREATE SEQUENCE teams_matches_id_seq START 1 INCREMENT BY 1;
CREATE TABLE IF NOT EXISTS public.teams_matches
(
    id integer NOT NULL DEFAULT nextval('teams_matches_id_seq'::regclass),
    id_team1 integer,
    score_team_1 integer,
    id_team_2 integer,
    score_team_2 integer,
    match_date timestamp without time zone,
    date_created timestamp without time zone,
    last_updated timestamp without time zone,
    CONSTRAINT teams_matches_pkey PRIMARY KEY (id),
    CONSTRAINT teams_matches_id_team1_fkey FOREIGN KEY (id_team1)
        REFERENCES public.team (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT teams_matches_id_team_2_fkey FOREIGN KEY (id_team_2)
        REFERENCES public.team (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);