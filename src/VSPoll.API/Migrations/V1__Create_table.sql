create table users (
    id integer primary key,
    first_name text not null,
    last_name text not null,
    username text not null,
    photo_url text not null
);

create table polls (
    id uuid primary key,
    description varchar(100) not null,
    multi_vote boolean not null default false,
    show_voters boolean not null default true,
    allow_add boolean not null default false,
    end_date timestamp without time zone not null default now() + '7 days'
);

create table poll_options (
    id uuid primary key,
    poll_id uuid not null references polls on delete cascade on update cascade,
    description varchar(100) not null,
    unique (poll_id, description)
);

create table poll_votes (
    option_id uuid not null references poll_options on delete cascade on update cascade,
    user_id integer not null references users on delete cascade on update cascade,
    constraint pk_poll_vote primary key (option_id, user_id)
);
