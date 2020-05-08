create table "user" (
    "id" text primary key,
    first_name text not null,
    last_name text not null,
    username text not null,
    photo_url text not null
);

create table poll (
    id integer generated always as identity primary key,
    multi_vote boolean not null default false,
    show_voters boolean not null default true,
    allow_add boolean not null default false,
    end_date timestamp without time zone not null default now() + '7 days',
    user_id text references "user" on update cascade on delete cascade
);

create table poll_block (
    poll_id integer not null references poll on delete cascade on update cascade,
    user_id text not null references "user" on delete cascade on update cascade,
    constraint pk_poll_block primary key (poll_id, user_id)
);

create table poll_option (
    id integer generated always as identity primary key,
    poll_id integer not null references poll on delete cascade on update cascade,
    description text not null,
    unique (poll_id, description)
);

create table poll_vote (
    option_id integer not null references poll_option on delete cascade on update cascade,
    user_id text not null references "user" on delete cascade on update cascade,
    constraint pk_poll_vote primary key (option_id, user_id)
);
