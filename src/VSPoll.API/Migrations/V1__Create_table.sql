create table poll (
    id integer generated always as identity primary key,
    multi_vote boolean not null default false,
    show_voters boolean not null default true,
    allow_add boolean not null default false,
    end_date timestamp without time zone not null default now() + '7 days',
    user text
);

create table poll_block (
    poll_id integer not null references poll on delete cascade on update cascade,
    user text not null,
    constraint pk_poll_block primary key (poll_id, user)
);

create table poll_option (
    id integer generated always as identity primary key,
    poll_id integer not null references poll on delete cascade on update cascade,
    description text not null,
    unique (poll_id, description)
);

create table poll_vote (
    option_id integer not null references poll_option on delete cascade on update cascade,
    user text not null,
    constraint pk_poll_vote primary key (option_id, user)
);
