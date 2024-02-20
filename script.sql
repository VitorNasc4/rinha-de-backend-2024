DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_tables WHERE schemaname = 'public' AND tablename = 'Clientes') THEN
        CREATE TABLE public."Clientes" (
            "Id" SERIAL PRIMARY KEY,
            "Saldo" DECIMAL NOT NULL,
            "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT now(),
            "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT now()
        );
    END IF;

    IF NOT EXISTS (SELECT FROM pg_catalog.pg_tables WHERE schemaname = 'public' AND tablename = 'Transacoes') THEN
        CREATE TABLE public."Transacoes" (
            "Id" SERIAL PRIMARY KEY,
            "Valor" DECIMAL NOT NULL,
            "Tipo" VARCHAR NOT NULL,
            "Descricao" VARCHAR NOT NULL,
            "IdCliente" INTEGER NOT NULL, 
            "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT now(),
            "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT now(),
            FOREIGN KEY ("IdCliente") REFERENCES public."Clientes"("Id")
        );
    END IF;
END $$;

INSERT INTO public."Clientes" ("Id", "Saldo", "CreatedAt", "UpdatedAt")
VALUES
    (1, 0, now(), now()),
    (2, 0, now(), now()),
    (3, 0, now(), now()),
    (4, 0, now(), now()),
    (5, 0, now(), now())
ON CONFLICT ("Id") 
DO NOTHING;
