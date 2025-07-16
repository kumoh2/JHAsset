CREATE TABLE asset (
    id           bigserial     PRIMARY KEY,          -- 1, 2, 3 … 자동 증가
    name         varchar(100)  NOT NULL UNIQUE,      -- 자산 이름
    ip_address   varchar(40)   NOT NULL,             -- IPv4 · IPv6
    port         int           NOT NULL CHECK (port BETWEEN 1 AND 65535),
    protocol     varchar(10)   NOT NULL DEFAULT 'tcp',
    description  text,
    is_active    boolean       NOT NULL DEFAULT true,
    created_at   timestamptz   NOT NULL DEFAULT now(),
    updated_at   timestamptz   NOT NULL DEFAULT now()
);