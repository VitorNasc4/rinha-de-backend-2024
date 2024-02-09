DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_tables WHERE schemaname = 'public' AND tablename = 'Clientes') THEN
        CREATE TABLE public."Clientes" (
            "Id" SERIAL PRIMARY KEY,
            "Limite" DECIMAL NOT NULL,
            "Saldo" DECIMAL NOT NULL,
            "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT now(),
            "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT now()
        );
    END IF;
END $$;

INSERT INTO public."Clientes" ("Id", "Limite", "Saldo", "CreatedAt", "UpdatedAt")
VALUES
    (1, 100000, 0, now(), now()),
    (2, 80000, 0, now(), now()),
    (3, 1000000, 0, now(), now()),
    (4, 10000000, 0, now(), now()),
    (5, 500000, 0, now(), now())
ON CONFLICT ("Id") 
DO NOTHING;
