--
-- PostgreSQL database dump
--

\restrict kB101P1Ux4eTRIkiCsep3nM51rfrPmOKWdNBnmBrViEFgi5JRqSl6HtmbxT9G2J

-- Dumped from database version 17.6
-- Dumped by pg_dump version 17.6

-- Started on 2026-06-28 20:04:06

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
-- TOC entry 875 (class 1247 OID 69670)
-- Name: account_type; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.account_type AS ENUM (
    'user',
    'company'
);


ALTER TYPE public.account_type OWNER TO postgres;

--
-- TOC entry 878 (class 1247 OID 69676)
-- Name: ingredient_unit; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.ingredient_unit AS ENUM (
    'g',
    'kg',
    'ml',
    'l',
    'pcs'
);


ALTER TYPE public.ingredient_unit OWNER TO postgres;

--
-- TOC entry 881 (class 1247 OID 69688)
-- Name: order_status; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.order_status AS ENUM (
    'awaiting',
    'in_progress',
    'completed',
    'canceled',
    'created'
);


ALTER TYPE public.order_status OWNER TO postgres;

--
-- TOC entry 884 (class 1247 OID 69700)
-- Name: task_status; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.task_status AS ENUM (
    'in_progress',
    'paused',
    'completed',
    'canceled'
);


ALTER TYPE public.task_status OWNER TO postgres;

--
-- TOC entry 887 (class 1247 OID 69710)
-- Name: user_role; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.user_role AS ENUM (
    'commercial_manager',
    'technologist'
);


ALTER TYPE public.user_role OWNER TO postgres;

--
-- TOC entry 890 (class 1247 OID 69716)
-- Name: verification_status; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.verification_status AS ENUM (
    'pending',
    'approved',
    'rejected',
    'suspended'
);


ALTER TYPE public.verification_status OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 217 (class 1259 OID 69725)
-- Name: accounts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.accounts (
    account_id integer NOT NULL,
    email character varying(255) NOT NULL,
    password_hash character varying(255) NOT NULL,
    account_type public.account_type NOT NULL,
    verification_status public.verification_status DEFAULT 'pending'::public.verification_status NOT NULL,
    session character varying(512),
    session_expires_at timestamp with time zone,
    deleted_at timestamp with time zone,
    created_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.accounts OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 69732)
-- Name: accounts_account_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.accounts_account_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.accounts_account_id_seq OWNER TO postgres;

--
-- TOC entry 5065 (class 0 OID 0)
-- Dependencies: 218
-- Name: accounts_account_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.accounts_account_id_seq OWNED BY public.accounts.account_id;


--
-- TOC entry 219 (class 1259 OID 69733)
-- Name: cart_items; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cart_items (
    cart_item_id integer NOT NULL,
    batch_id integer,
    quantity integer NOT NULL,
    company_id integer
);


ALTER TABLE public.cart_items OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 69736)
-- Name: cart_items_new_cart_item_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cart_items_new_cart_item_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cart_items_new_cart_item_id_seq OWNER TO postgres;

--
-- TOC entry 5066 (class 0 OID 0)
-- Dependencies: 220
-- Name: cart_items_new_cart_item_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cart_items_new_cart_item_id_seq OWNED BY public.cart_items.cart_item_id;


--
-- TOC entry 221 (class 1259 OID 69737)
-- Name: companies; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.companies (
    company_id integer NOT NULL,
    account_id integer NOT NULL,
    name character varying(150) NOT NULL,
    inn character varying(10) NOT NULL,
    ogrn character varying(13) NOT NULL,
    phone character varying(11),
    address text
);


ALTER TABLE public.companies OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 69742)
-- Name: companies_company_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.companies_company_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.companies_company_id_seq OWNER TO postgres;

--
-- TOC entry 5067 (class 0 OID 0)
-- Dependencies: 222
-- Name: companies_company_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.companies_company_id_seq OWNED BY public.companies.company_id;


--
-- TOC entry 223 (class 1259 OID 69743)
-- Name: employee_tasks; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.employee_tasks (
    employee_task_id integer NOT NULL,
    employee_id integer NOT NULL,
    order_item_id integer NOT NULL,
    assigned_quantity integer NOT NULL,
    completed_quantity integer DEFAULT 0,
    start_time timestamp with time zone,
    end_time timestamp with time zone,
    status public.task_status DEFAULT 'in_progress'::public.task_status NOT NULL,
    CONSTRAINT employee_tasks_new_assigned_quantity_check CHECK ((assigned_quantity > 0)),
    CONSTRAINT employee_tasks_new_completed_quantity_check CHECK ((completed_quantity >= 0))
);


ALTER TABLE public.employee_tasks OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 69750)
-- Name: employee_tasks_new_employee_task_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.employee_tasks_new_employee_task_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.employee_tasks_new_employee_task_id_seq OWNER TO postgres;

--
-- TOC entry 5068 (class 0 OID 0)
-- Dependencies: 224
-- Name: employee_tasks_new_employee_task_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employee_tasks_new_employee_task_id_seq OWNED BY public.employee_tasks.employee_task_id;


--
-- TOC entry 225 (class 1259 OID 69751)
-- Name: employees; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.employees (
    employee_id integer NOT NULL,
    firstname character varying(50) NOT NULL,
    lastname character varying(50) NOT NULL,
    patronymic character varying(50),
    birthday date NOT NULL,
    deleted_at timestamp with time zone
);


ALTER TABLE public.employees OWNER TO postgres;

--
-- TOC entry 226 (class 1259 OID 69754)
-- Name: employees_employee_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.employees_employee_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.employees_employee_id_seq OWNER TO postgres;

--
-- TOC entry 5069 (class 0 OID 0)
-- Dependencies: 226
-- Name: employees_employee_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employees_employee_id_seq OWNED BY public.employees.employee_id;


--
-- TOC entry 227 (class 1259 OID 69755)
-- Name: favorites; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.favorites (
    favorite_id integer NOT NULL,
    product_id integer NOT NULL,
    company_id integer
);


ALTER TABLE public.favorites OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 69758)
-- Name: favourites_favourite_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.favourites_favourite_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.favourites_favourite_id_seq OWNER TO postgres;

--
-- TOC entry 5070 (class 0 OID 0)
-- Dependencies: 228
-- Name: favourites_favourite_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.favourites_favourite_id_seq OWNED BY public.favorites.favorite_id;


--
-- TOC entry 229 (class 1259 OID 69759)
-- Name: ingredients; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ingredients (
    ingredient_id integer NOT NULL,
    name character varying(100) NOT NULL,
    deleted_at timestamp with time zone,
    base_unit public.ingredient_unit NOT NULL
);


ALTER TABLE public.ingredients OWNER TO postgres;

--
-- TOC entry 230 (class 1259 OID 69762)
-- Name: ingredients_ingredient_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.ingredients_ingredient_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.ingredients_ingredient_id_seq OWNER TO postgres;

--
-- TOC entry 5071 (class 0 OID 0)
-- Dependencies: 230
-- Name: ingredients_ingredient_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredients_ingredient_id_seq OWNED BY public.ingredients.ingredient_id;


--
-- TOC entry 231 (class 1259 OID 69763)
-- Name: order_items; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.order_items (
    order_item_id integer NOT NULL,
    order_id integer NOT NULL,
    batch_id integer NOT NULL,
    quantity integer NOT NULL,
    unit_price numeric(10,2) DEFAULT 0 NOT NULL,
    units_per_batch integer NOT NULL
);


ALTER TABLE public.order_items OWNER TO postgres;

--
-- TOC entry 232 (class 1259 OID 69767)
-- Name: order_items_new_order_item_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.order_items_new_order_item_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.order_items_new_order_item_id_seq OWNER TO postgres;

--
-- TOC entry 5072 (class 0 OID 0)
-- Dependencies: 232
-- Name: order_items_new_order_item_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.order_items_new_order_item_id_seq OWNED BY public.order_items.order_item_id;


--
-- TOC entry 233 (class 1259 OID 69768)
-- Name: orders; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.orders (
    order_id integer NOT NULL,
    status public.order_status NOT NULL,
    start_date date,
    end_date date NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    company_id integer NOT NULL,
    canceled_at timestamp with time zone
);


ALTER TABLE public.orders OWNER TO postgres;

--
-- TOC entry 234 (class 1259 OID 69772)
-- Name: orders_order_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.orders_order_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.orders_order_id_seq OWNER TO postgres;

--
-- TOC entry 5073 (class 0 OID 0)
-- Dependencies: 234
-- Name: orders_order_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.orders_order_id_seq OWNED BY public.orders.order_id;


--
-- TOC entry 235 (class 1259 OID 69773)
-- Name: product_batches; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product_batches (
    product_batch_id integer NOT NULL,
    product_id integer NOT NULL,
    quantity_units integer NOT NULL,
    markup_percent integer DEFAULT 0 NOT NULL,
    CONSTRAINT product_batches_new_quantity_check CHECK ((quantity_units > 0))
);


ALTER TABLE public.product_batches OWNER TO postgres;

--
-- TOC entry 236 (class 1259 OID 69778)
-- Name: product_batches_new_product_batch_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.product_batches_new_product_batch_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.product_batches_new_product_batch_id_seq OWNER TO postgres;

--
-- TOC entry 5074 (class 0 OID 0)
-- Dependencies: 236
-- Name: product_batches_new_product_batch_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.product_batches_new_product_batch_id_seq OWNED BY public.product_batches.product_batch_id;


--
-- TOC entry 237 (class 1259 OID 69779)
-- Name: product_categories; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product_categories (
    product_category_id integer NOT NULL,
    name character varying(100) NOT NULL,
    color character varying(6) NOT NULL,
    deleted_at timestamp with time zone
);


ALTER TABLE public.product_categories OWNER TO postgres;

--
-- TOC entry 238 (class 1259 OID 69782)
-- Name: product_images; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product_images (
    product_image_id integer NOT NULL,
    product_id integer NOT NULL,
    image_path character varying
);


ALTER TABLE public.product_images OWNER TO postgres;

--
-- TOC entry 239 (class 1259 OID 69787)
-- Name: product_images_product_image_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.product_images_product_image_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.product_images_product_image_id_seq OWNER TO postgres;

--
-- TOC entry 5075 (class 0 OID 0)
-- Dependencies: 239
-- Name: product_images_product_image_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.product_images_product_image_id_seq OWNED BY public.product_images.product_image_id;


--
-- TOC entry 240 (class 1259 OID 69788)
-- Name: production_categories_production_category_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.production_categories_production_category_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.production_categories_production_category_id_seq OWNER TO postgres;

--
-- TOC entry 5076 (class 0 OID 0)
-- Dependencies: 240
-- Name: production_categories_production_category_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.production_categories_production_category_id_seq OWNED BY public.product_categories.product_category_id;


--
-- TOC entry 241 (class 1259 OID 69789)
-- Name: products; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.products (
    product_id integer NOT NULL,
    category_id integer NOT NULL,
    name character varying(100) NOT NULL,
    description text NOT NULL,
    cost_price numeric(10,2) NOT NULL,
    weight numeric(10,2) NOT NULL,
    production_time_minutes integer NOT NULL,
    deleted_at timestamp with time zone,
    storage_temp_min numeric(10,2) DEFAULT 2.0,
    storage_temp_max numeric(10,2) DEFAULT 6.0,
    shelf_life_days integer DEFAULT 3,
    created_at timestamp with time zone DEFAULT now()
);


ALTER TABLE public.products OWNER TO postgres;

--
-- TOC entry 242 (class 1259 OID 69798)
-- Name: products_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.products_product_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.products_product_id_seq OWNER TO postgres;

--
-- TOC entry 5077 (class 0 OID 0)
-- Dependencies: 242
-- Name: products_product_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.products_product_id_seq OWNED BY public.products.product_id;


--
-- TOC entry 243 (class 1259 OID 69799)
-- Name: recipes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.recipes (
    recipe_id integer NOT NULL,
    product_id integer NOT NULL,
    ingredient_id integer NOT NULL,
    quantity numeric(4,3) NOT NULL
);


ALTER TABLE public.recipes OWNER TO postgres;

--
-- TOC entry 244 (class 1259 OID 69802)
-- Name: recipes_recipe_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.recipes_recipe_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.recipes_recipe_id_seq OWNER TO postgres;

--
-- TOC entry 5078 (class 0 OID 0)
-- Dependencies: 244
-- Name: recipes_recipe_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.recipes_recipe_id_seq OWNED BY public.recipes.recipe_id;


--
-- TOC entry 245 (class 1259 OID 69803)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    user_id integer NOT NULL,
    account_id integer NOT NULL,
    firstname character varying(50) NOT NULL,
    lastname character varying(50) NOT NULL,
    patronymic character varying(50),
    birthday date NOT NULL,
    role public.user_role NOT NULL
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 246 (class 1259 OID 69806)
-- Name: system_users_user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.system_users_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.system_users_user_id_seq OWNER TO postgres;

--
-- TOC entry 5079 (class 0 OID 0)
-- Dependencies: 246
-- Name: system_users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.system_users_user_id_seq OWNED BY public.users.user_id;


--
-- TOC entry 4783 (class 2604 OID 69807)
-- Name: accounts account_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts ALTER COLUMN account_id SET DEFAULT nextval('public.accounts_account_id_seq'::regclass);


--
-- TOC entry 4786 (class 2604 OID 69808)
-- Name: cart_items cart_item_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items ALTER COLUMN cart_item_id SET DEFAULT nextval('public.cart_items_new_cart_item_id_seq'::regclass);


--
-- TOC entry 4787 (class 2604 OID 69809)
-- Name: companies company_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies ALTER COLUMN company_id SET DEFAULT nextval('public.companies_company_id_seq'::regclass);


--
-- TOC entry 4788 (class 2604 OID 69810)
-- Name: employee_tasks employee_task_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks ALTER COLUMN employee_task_id SET DEFAULT nextval('public.employee_tasks_new_employee_task_id_seq'::regclass);


--
-- TOC entry 4791 (class 2604 OID 69811)
-- Name: employees employee_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees ALTER COLUMN employee_id SET DEFAULT nextval('public.employees_employee_id_seq'::regclass);


--
-- TOC entry 4792 (class 2604 OID 69812)
-- Name: favorites favorite_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites ALTER COLUMN favorite_id SET DEFAULT nextval('public.favourites_favourite_id_seq'::regclass);


--
-- TOC entry 4793 (class 2604 OID 69813)
-- Name: ingredients ingredient_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients ALTER COLUMN ingredient_id SET DEFAULT nextval('public.ingredients_ingredient_id_seq'::regclass);


--
-- TOC entry 4794 (class 2604 OID 69814)
-- Name: order_items order_item_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items ALTER COLUMN order_item_id SET DEFAULT nextval('public.order_items_new_order_item_id_seq'::regclass);


--
-- TOC entry 4796 (class 2604 OID 69815)
-- Name: orders order_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders ALTER COLUMN order_id SET DEFAULT nextval('public.orders_order_id_seq'::regclass);


--
-- TOC entry 4798 (class 2604 OID 69816)
-- Name: product_batches product_batch_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches ALTER COLUMN product_batch_id SET DEFAULT nextval('public.product_batches_new_product_batch_id_seq'::regclass);


--
-- TOC entry 4800 (class 2604 OID 69817)
-- Name: product_categories product_category_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_categories ALTER COLUMN product_category_id SET DEFAULT nextval('public.production_categories_production_category_id_seq'::regclass);


--
-- TOC entry 4801 (class 2604 OID 69818)
-- Name: product_images product_image_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images ALTER COLUMN product_image_id SET DEFAULT nextval('public.product_images_product_image_id_seq'::regclass);


--
-- TOC entry 4802 (class 2604 OID 69819)
-- Name: products product_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products ALTER COLUMN product_id SET DEFAULT nextval('public.products_product_id_seq'::regclass);


--
-- TOC entry 4807 (class 2604 OID 69820)
-- Name: recipes recipe_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes ALTER COLUMN recipe_id SET DEFAULT nextval('public.recipes_recipe_id_seq'::regclass);


--
-- TOC entry 4808 (class 2604 OID 69821)
-- Name: users user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.system_users_user_id_seq'::regclass);


--
-- TOC entry 5030 (class 0 OID 69725)
-- Dependencies: 217
-- Data for Name: accounts; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.accounts (account_id, email, password_hash, account_type, verification_status, session, session_expires_at, deleted_at, created_at) FROM stdin;
4	restoria@gmail.com	$2a$11$uxWmFAglkbQewHliiUFXMu7YruHxDE69VTZugw5DzNauQS3gsxm96	company	pending	78024ea9ecd743e7ab50d5e6c06b1749	2026-05-23 13:45:38.430474+05	\N	2026-05-22 10:38:55.658091+05
1	company@bakery.ru	$2a$11$mBkW8NaQM.Kc0/lvS1ik/.U3fgSBa98OmeIozzq9a.8AHpPvE4F4W	company	approved	d65bcb272c354196a16b89c89c4abde8	2026-06-28 22:58:10.618074+05	\N	2026-04-01 00:00:00+05
8	asdf@fasd.eqe	$2a$11$K9Z/db76JgxIkh9OSwAbJOTzID7DSuo0hXLflcspzXPZKxOEF0JL.	company	pending	eb5f05bc0ede4c9a803cf6ba01d9ac16	2026-06-14 17:03:13.610631+05	\N	2026-06-13 16:57:56.032475+05
5	maslo@mail.ru	$2a$11$spU.V/t.o2XxHVqNlJJLW.37WuzkKJAnhxwciek6OtPBIMyXdx.7S	company	rejected	\N	\N	\N	2026-05-22 10:39:51.629287+05
6	buhanka@gmail.com	$2a$11$3BcNqlHFug9rN42V0eL08u2xQwUMyQHXHl0Y7d78E6TK6jBi8DYGe	company	pending	\N	\N	2026-06-05 18:48:00.56514+05	2026-06-05 18:47:55.486332+05
7	barakat@gmail.com	$2a$11$QJUU80z39QrzDWLkzy/RXO6RTXQFx9.AfYVK5u5xV8VTa3tVBQlWK	company	approved	d64aa92c75ad499e9a441c31ddc43cd4	2026-06-06 19:08:58.928563+05	\N	2026-06-05 19:02:19.568708+05
9	alrkc@gmail.com	$2a$11$58/siUo5HlBLUQdc.zYJE.uAvCn5qDbfPOfUQ/uRSsf4.2mWVsVQG	user	approved	\N	\N	\N	2026-06-13 20:00:20.505544+05
3	comm@bakery.ru	$2a$11$29Kij2EN7VlJm8gbOPR5TudpWZHKqBeoNc8cWVLVBRiA7wHVQSDvK	user	approved	\N	\N	\N	2026-04-01 00:00:00+05
2	tech@bakery.ru	$2a$11$qWbk77OYChBocx14lkAMBe0xe3hDL09aA8og/el9RCugaSYA8EUY.	user	approved	9926763b6e6949cf85de35cd7cade075	2026-06-28 22:57:57.518993+05	\N	2026-04-01 00:00:00+05
\.


--
-- TOC entry 5032 (class 0 OID 69733)
-- Dependencies: 219
-- Data for Name: cart_items; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.cart_items (cart_item_id, batch_id, quantity, company_id) FROM stdin;
\.


--
-- TOC entry 5034 (class 0 OID 69737)
-- Dependencies: 221
-- Data for Name: companies; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.companies (company_id, account_id, name, inn, ogrn, phone, address) FROM stdin;
2	4	ООО "Рестория"	1321918231	5254123412341	89212313212	\N
3	5	"Сыр в масле"	1419123193	5213391231412	\N	\N
4	6	ООО "Свежая буханка"	1244143234	1232142321312	\N	\N
5	7	Кафе Баракат	1341432312	1544353345345	\N	\N
6	8	Asdf	1232412413	5134134234123	\N	\N
1	1	ООО "Хлебное Место"	5793851130	5264386528962	89371234134	г. Уфа, Демский р-н, ул. Новоселова 10
\.


--
-- TOC entry 5036 (class 0 OID 69743)
-- Dependencies: 223
-- Data for Name: employee_tasks; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employee_tasks (employee_task_id, employee_id, order_item_id, assigned_quantity, completed_quantity, start_time, end_time, status) FROM stdin;
1	1	2	1	0	\N	\N	completed
2	2	2	1	0	\N	\N	completed
3	1	1	1	0	\N	\N	completed
4	2	1	1	0	\N	\N	completed
5	1	5	2	0	2026-05-28 16:20:00+05	2026-05-28 17:00:00+05	completed
6	2	5	2	0	2026-05-28 16:20:00+05	2026-05-28 17:00:00+05	completed
7	4	5	2	0	2026-05-28 16:20:00+05	2026-05-28 17:00:00+05	completed
8	1	4	1	0	2026-05-28 15:50:00+05	2026-05-28 16:20:00+05	completed
9	2	4	1	0	2026-05-28 15:50:00+05	2026-05-28 16:20:00+05	completed
10	4	4	2	0	2026-05-28 15:20:00+05	2026-05-28 16:20:00+05	completed
11	1	6	2	0	2026-05-28 14:50:00+05	2026-05-28 15:50:00+05	completed
12	2	6	2	0	2026-05-28 14:50:00+05	2026-05-28 15:50:00+05	completed
13	4	6	2	0	2026-05-28 14:20:00+05	2026-05-28 15:20:00+05	completed
14	6	12	2	2	2026-06-03 16:20:00+05	2026-06-03 17:00:00+05	paused
24	1	16	1	0	2026-06-04 16:30:00+05	2026-06-04 17:00:00+05	in_progress
26	4	16	1	0	2026-06-04 16:30:00+05	2026-06-04 17:00:00+05	in_progress
27	5	16	1	0	2026-06-04 16:30:00+05	2026-06-04 17:00:00+05	in_progress
28	6	16	2	0	2026-06-04 16:00:00+05	2026-06-04 17:00:00+05	in_progress
15	1	13	2	2	2026-06-05 16:00:00+05	2026-06-05 17:00:00+05	completed
16	2	13	2	2	2026-06-05 16:00:00+05	2026-06-05 17:00:00+05	completed
19	1	14	3	0	2026-06-11 16:00:00+05	2026-06-11 17:00:00+05	canceled
20	2	14	3	0	2026-06-11 16:00:00+05	2026-06-11 17:00:00+05	canceled
21	4	14	3	0	2026-06-11 16:00:00+05	2026-06-11 17:00:00+05	canceled
22	5	14	3	0	2026-06-11 16:00:00+05	2026-06-11 17:00:00+05	canceled
23	6	14	4	0	2026-06-11 15:40:00+05	2026-06-11 17:00:00+05	canceled
29	1	18	1	0	2026-06-04 16:00:00+05	2026-06-04 16:30:00+05	in_progress
30	2	18	1	0	2026-06-04 16:00:00+05	2026-06-04 16:30:00+05	in_progress
31	4	18	1	0	2026-06-04 16:00:00+05	2026-06-04 16:30:00+05	in_progress
32	5	18	1	0	2026-06-04 16:00:00+05	2026-06-04 16:30:00+05	in_progress
33	6	17	1	0	2026-06-04 15:15:00+05	2026-06-04 16:00:00+05	in_progress
34	1	17	1	0	2026-06-04 15:15:00+05	2026-06-04 16:00:00+05	in_progress
35	2	17	1	0	2026-06-04 15:15:00+05	2026-06-04 16:00:00+05	in_progress
25	2	16	1	1	2026-06-04 16:30:00+05	2026-06-04 17:00:00+05	paused
38	4	20	1	0	2026-06-26 16:30:00+05	2026-06-26 17:00:00+05	in_progress
41	7	20	1	0	2026-06-26 16:30:00+05	2026-06-26 17:00:00+05	in_progress
42	7	22	2	0	2026-06-26 15:30:00+05	2026-06-26 16:30:00+05	in_progress
43	1	21	2	0	2026-06-26 14:40:00+05	2026-06-26 16:30:00+05	in_progress
45	4	21	2	0	2026-06-26 14:40:00+05	2026-06-26 16:30:00+05	in_progress
47	6	21	1	0	2026-06-26 15:35:00+05	2026-06-26 16:30:00+05	in_progress
17	4	13	2	2	2026-06-05 16:00:00+05	2026-06-05 17:00:00+05	paused
18	5	13	2	2	2026-06-05 16:00:00+05	2026-06-05 17:00:00+05	completed
36	1	20	1	1	2026-06-26 16:30:00+05	2026-06-26 17:00:00+05	paused
37	2	20	1	1	2026-06-26 16:30:00+05	2026-06-26 17:00:00+05	completed
44	2	21	2	2	2026-06-26 14:40:00+05	2026-06-26 16:30:00+05	in_progress
49	2	26	1	0	2026-06-16 16:30:00+05	2026-06-16 17:00:00+05	in_progress
50	4	26	1	0	2026-06-16 16:30:00+05	2026-06-16 17:00:00+05	in_progress
51	5	26	1	0	2026-06-16 16:30:00+05	2026-06-16 17:00:00+05	in_progress
39	5	20	1	1	2026-06-26 16:30:00+05	2026-06-26 17:00:00+05	completed
40	6	20	1	0	2026-06-26 16:30:00+05	2026-06-26 17:00:00+05	paused
46	5	21	2	0	2026-06-26 14:40:00+05	2026-06-26 16:30:00+05	in_progress
48	1	26	1	1	2026-06-16 16:30:00+05	2026-06-16 17:00:00+05	in_progress
52	1	34	6	0	2026-06-22 14:00:00+05	2026-06-22 17:00:00+05	in_progress
53	2	34	6	0	2026-06-22 14:00:00+05	2026-06-22 17:00:00+05	in_progress
54	4	34	6	0	2026-06-22 14:00:00+05	2026-06-22 17:00:00+05	in_progress
55	5	34	6	0	2026-06-22 14:00:00+05	2026-06-22 17:00:00+05	in_progress
56	6	34	6	0	2026-06-22 14:00:00+05	2026-06-22 17:00:00+05	in_progress
57	7	34	6	0	2026-06-22 14:00:00+05	2026-06-22 17:00:00+05	in_progress
58	1	33	1	0	2026-06-22 13:15:00+05	2026-06-22 14:00:00+05	in_progress
59	2	33	1	0	2026-06-22 13:15:00+05	2026-06-22 14:00:00+05	in_progress
60	4	33	1	0	2026-06-22 13:15:00+05	2026-06-22 14:00:00+05	in_progress
61	5	33	1	0	2026-06-22 13:15:00+05	2026-06-22 14:00:00+05	in_progress
62	6	33	1	0	2026-06-22 13:15:00+05	2026-06-22 14:00:00+05	in_progress
63	7	33	1	0	2026-06-22 13:15:00+05	2026-06-22 14:00:00+05	in_progress
64	1	32	1	0	2026-06-22 11:30:00+05	2026-06-22 12:00:00+05	in_progress
65	2	32	1	0	2026-06-22 11:30:00+05	2026-06-22 12:00:00+05	in_progress
66	4	32	1	0	2026-06-22 11:30:00+05	2026-06-22 12:00:00+05	in_progress
67	5	32	1	0	2026-06-22 11:30:00+05	2026-06-22 12:00:00+05	in_progress
68	6	32	1	0	2026-06-22 11:30:00+05	2026-06-22 12:00:00+05	in_progress
69	7	32	1	0	2026-06-22 11:30:00+05	2026-06-22 12:00:00+05	in_progress
70	1	31	1	0	2026-06-22 10:45:00+05	2026-06-22 11:30:00+05	in_progress
71	2	31	1	0	2026-06-22 10:45:00+05	2026-06-22 11:30:00+05	in_progress
72	4	31	1	0	2026-06-22 10:45:00+05	2026-06-22 11:30:00+05	in_progress
73	5	31	1	0	2026-06-22 10:45:00+05	2026-06-22 11:30:00+05	in_progress
\.


--
-- TOC entry 5038 (class 0 OID 69751)
-- Dependencies: 225
-- Data for Name: employees; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employees (employee_id, firstname, lastname, patronymic, birthday, deleted_at) FROM stdin;
1	Иван	Петров	Сергеевич	1985-01-10	\N
2	Петр	Васильев	Алексеевич	1987-05-20	\N
4	Лев	Григориев	Николаевич	2001-01-01	\N
5	Фывва	Фыва	\N	1968-05-01	\N
6	Фывава	Фыв	\N	1962-01-01	\N
7	Игорь	Николаев	\N	1963-01-01	\N
\.


--
-- TOC entry 5040 (class 0 OID 69755)
-- Dependencies: 227
-- Data for Name: favorites; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.favorites (favorite_id, product_id, company_id) FROM stdin;
1	3	1
2	1	1
3	2	1
4	4	1
5	6	1
\.


--
-- TOC entry 5042 (class 0 OID 69759)
-- Dependencies: 229
-- Data for Name: ingredients; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredients (ingredient_id, name, deleted_at, base_unit) FROM stdin;
2	Сахар-песок	\N	kg
3	Масло сливочное	\N	kg
4	Яйца куриные	\N	g
5	фыв	2026-06-02 02:58:37.997816+05	ml
1	Мука пшеничная	\N	kg
6	Дрожжи	\N	kg
7	Мука	\N	g
\.


--
-- TOC entry 5044 (class 0 OID 69763)
-- Dependencies: 231
-- Data for Name: order_items; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.order_items (order_item_id, order_id, batch_id, quantity, unit_price, units_per_batch) FROM stdin;
1	1	1	1	35.00	2
2	1	2	1	110.50	2
7	6	6	7	336.00	2
8	6	4	4	108.80	4
9	6	7	1	288.00	4
6	5	1	3	35.00	2
4	4	2	2	110.50	2
5	4	6	3	336.00	2
10	7	1	1	35.00	2
11	7	4	2	108.80	4
12	8	6	1	336.00	2
13	9	1	4	35.00	2
14	10	7	4	288.00	4
15	11	4	2	108.80	4
16	12	1	3	35.00	2
17	13	3	1	78.00	3
18	13	4	1	108.80	4
19	14	8	3	225.00	3
20	15	1	3	35.00	2
21	15	8	3	225.00	3
22	15	2	1	110.50	2
23	16	8	1	225.00	3
24	16	1	2	35.00	2
25	16	4	1	108.80	4
26	17	1	2	35.00	2
27	18	1	1	35.00	2
28	18	10	3	95.20	6
29	18	8	1	225.00	3
30	18	6	5	336.00	2
31	19	5	2	91.00	2
32	19	1	3	35.00	2
33	20	5	3	91.00	2
34	20	10	6	95.20	6
35	21	1	4	35.00	2
36	22	2	3	110.50	2
\.


--
-- TOC entry 5046 (class 0 OID 69768)
-- Dependencies: 233
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orders (order_id, status, start_date, end_date, created_at, company_id, canceled_at) FROM stdin;
1	completed	2026-04-03	2026-04-03	2026-04-01 10:30:00+05	1	\N
4	completed	2026-05-28	2026-05-28	2026-05-22 11:08:35.458006+05	2	\N
5	completed	2026-05-28	2026-05-28	2026-05-22 13:32:25.99706+05	1	\N
7	canceled	\N	2026-05-27	2026-05-22 13:46:28.964914+05	3	2026-06-01 22:51:19.466981+05
6	canceled	\N	2026-05-27	2026-05-22 13:35:36.710257+05	1	2026-06-01 22:51:21.866511+05
8	completed	2026-06-03	2026-06-03	2026-06-01 22:38:39.80045+05	2	\N
9	canceled	2026-06-05	2026-06-05	2026-06-02 02:00:15.474742+05	2	2026-06-02 02:03:32.170945+05
11	canceled	\N	2026-06-13	2026-06-02 02:05:50.755442+05	1	2026-06-02 02:06:04.719677+05
10	canceled	2026-06-11	2026-06-11	2026-06-02 02:04:34.695167+05	1	2026-06-02 02:06:13.583997+05
12	in_progress	2026-06-04	2026-06-04	2026-06-02 02:15:13.435136+05	3	\N
13	in_progress	2026-06-04	2026-06-04	2026-06-02 06:50:52.150118+05	1	\N
14	created	\N	2026-06-13	2026-06-05 18:17:30.465796+05	2	\N
15	in_progress	2026-06-26	2026-06-26	2026-06-05 18:45:19.343187+05	1	\N
16	created	\N	2026-06-09	2026-06-05 19:00:10.256031+05	1	\N
17	in_progress	2026-06-16	2026-06-16	2026-06-11 17:34:44.849334+05	3	\N
18	created	\N	2026-06-16	2026-06-11 17:36:10.436469+05	1	\N
20	in_progress	2026-06-22	2026-06-22	2026-06-13 19:47:36.251211+05	1	\N
19	in_progress	2026-06-22	2026-06-22	2026-06-13 19:46:12.012581+05	1	\N
21	created	\N	2026-06-18	2026-06-13 19:59:02.376303+05	2	\N
22	created	\N	2026-06-29	2026-06-26 17:46:59.496563+05	1	\N
\.


--
-- TOC entry 5048 (class 0 OID 69773)
-- Dependencies: 235
-- Data for Name: product_batches; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_batches (product_batch_id, product_id, quantity_units, markup_percent) FROM stdin;
1	1	2	25
2	2	2	30
3	3	3	20
4	2	4	28
5	3	2	40
6	4	2	40
7	4	4	20
8	5	3	50
9	6	6	60
10	2	6	12
\.


--
-- TOC entry 5050 (class 0 OID 69779)
-- Dependencies: 237
-- Data for Name: product_categories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_categories (product_category_id, name, color, deleted_at) FROM stdin;
3	Сладкое	123234	2026-06-02 05:08:16.17905+05
2	Пирожные	FF69B4	\N
1	Выпечка свежая	FFD700	\N
4	Пироги	874121	\N
5	Слоеная выпечка	099340	\N
6	Пирожные	FF0000	2026-06-13 16:24:04.150068+05
7	Выпечка	019421	2026-06-27 22:58:05.157615+05
\.


--
-- TOC entry 5051 (class 0 OID 69782)
-- Dependencies: 238
-- Data for Name: product_images; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_images (product_image_id, product_id, image_path) FROM stdin;
1	1	sdobnaya_bulocha_s_koricei.jpg
2	2	ecler_s_zavarnym_kremom_prev.jpg
3	2	ecler_s_zavarnym_kremom_2.jpg
4	5	6f1c6e9fff764a2f9549ba6131067240.jpg
5	6	a5b298660faf45e1bfe23d687e5313c7.jpg
7	3	59e4644557e74d7aafb5e678fa914a97.png
\.


--
-- TOC entry 5054 (class 0 OID 69789)
-- Dependencies: 241
-- Data for Name: products; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.products (product_id, category_id, name, description, cost_price, weight, production_time_minutes, deleted_at, storage_temp_min, storage_temp_max, shelf_life_days, created_at) FROM stdin;
3	1	Круассан масляный	Слоеный круассан	65.00	0.07	45	\N	18.00	22.00	2	2026-06-02 06:52:27.917627+05
2	5	Эклер с заварным кремом	Заварное пирожное	85.00	0.08	30	\N	2.00	6.00	3	2026-06-02 06:52:27.917627+05
5	4	Яблочный пирог	Вкусный слоеный пирог из нежного теста и сливок	210.00	30.00	55	\N	-2.00	4.00	7	-infinity
6	2	Кекс шоколадный	Очень вкусный	100.00	30.00	20	\N	0.00	0.00	3	-infinity
1	1	Сдобная булочка с корицей	Сладкая булочка с корицей	28.00	0.09	30	\N	18.00	25.00	2	2026-06-02 06:52:27.917627+05
4	1	Шоколадный чизкейк	С молочкно-шоколадной нежной начинкой, десерт на 320кк	240.00	170.00	20	\N	-6.00	12.00	4	2026-06-02 06:52:27.917627+05
\.


--
-- TOC entry 5056 (class 0 OID 69799)
-- Dependencies: 243
-- Data for Name: recipes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.recipes (recipe_id, product_id, ingredient_id, quantity) FROM stdin;
1	1	1	0.250
2	1	2	0.040
3	1	3	0.020
4	1	4	1.000
8	3	1	0.300
9	3	3	0.100
10	3	2	0.020
14	4	2	0.200
15	4	1	0.200
16	4	3	0.200
17	5	3	0.500
18	5	2	0.500
19	5	1	0.500
20	6	3	0.200
21	6	1	0.300
22	6	4	0.700
23	2	1	0.200
24	2	3	0.050
25	2	4	2.000
26	2	2	0.300
\.


--
-- TOC entry 5058 (class 0 OID 69803)
-- Dependencies: 245
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (user_id, account_id, firstname, lastname, patronymic, birthday, role) FROM stdin;
2	3	Марина	Соколова	Петровна	1992-07-22	commercial_manager
1	2	Алексей	Воробьев	Иванович	1988-03-15	technologist
3	9	Алексей	Николаев	\N	1974-01-01	commercial_manager
\.


--
-- TOC entry 5080 (class 0 OID 0)
-- Dependencies: 218
-- Name: accounts_account_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.accounts_account_id_seq', 9, true);


--
-- TOC entry 5081 (class 0 OID 0)
-- Dependencies: 220
-- Name: cart_items_new_cart_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.cart_items_new_cart_item_id_seq', 20, true);


--
-- TOC entry 5082 (class 0 OID 0)
-- Dependencies: 222
-- Name: companies_company_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.companies_company_id_seq', 6, true);


--
-- TOC entry 5083 (class 0 OID 0)
-- Dependencies: 224
-- Name: employee_tasks_new_employee_task_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employee_tasks_new_employee_task_id_seq', 73, true);


--
-- TOC entry 5084 (class 0 OID 0)
-- Dependencies: 226
-- Name: employees_employee_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employees_employee_id_seq', 7, true);


--
-- TOC entry 5085 (class 0 OID 0)
-- Dependencies: 228
-- Name: favourites_favourite_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.favourites_favourite_id_seq', 5, true);


--
-- TOC entry 5086 (class 0 OID 0)
-- Dependencies: 230
-- Name: ingredients_ingredient_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredients_ingredient_id_seq', 7, true);


--
-- TOC entry 5087 (class 0 OID 0)
-- Dependencies: 232
-- Name: order_items_new_order_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.order_items_new_order_item_id_seq', 36, true);


--
-- TOC entry 5088 (class 0 OID 0)
-- Dependencies: 234
-- Name: orders_order_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orders_order_id_seq', 22, true);


--
-- TOC entry 5089 (class 0 OID 0)
-- Dependencies: 236
-- Name: product_batches_new_product_batch_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.product_batches_new_product_batch_id_seq', 11, true);


--
-- TOC entry 5090 (class 0 OID 0)
-- Dependencies: 239
-- Name: product_images_product_image_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.product_images_product_image_id_seq', 7, true);


--
-- TOC entry 5091 (class 0 OID 0)
-- Dependencies: 240
-- Name: production_categories_production_category_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.production_categories_production_category_id_seq', 7, true);


--
-- TOC entry 5092 (class 0 OID 0)
-- Dependencies: 242
-- Name: products_product_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.products_product_id_seq', 7, true);


--
-- TOC entry 5093 (class 0 OID 0)
-- Dependencies: 244
-- Name: recipes_recipe_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.recipes_recipe_id_seq', 29, true);


--
-- TOC entry 5094 (class 0 OID 0)
-- Dependencies: 246
-- Name: system_users_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.system_users_user_id_seq', 3, true);


--
-- TOC entry 4814 (class 2606 OID 69823)
-- Name: accounts accounts_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts
    ADD CONSTRAINT accounts_pkey PRIMARY KEY (account_id);


--
-- TOC entry 4816 (class 2606 OID 69825)
-- Name: cart_items cart_items_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT cart_items_new_pkey PRIMARY KEY (cart_item_id);


--
-- TOC entry 4820 (class 2606 OID 69827)
-- Name: companies companies_inn_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_inn_key UNIQUE (inn);


--
-- TOC entry 4822 (class 2606 OID 69829)
-- Name: companies companies_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_name_key UNIQUE (name);


--
-- TOC entry 4824 (class 2606 OID 69831)
-- Name: companies companies_ogrn_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_ogrn_key UNIQUE (ogrn);


--
-- TOC entry 4826 (class 2606 OID 69833)
-- Name: companies companies_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_pkey PRIMARY KEY (company_id);


--
-- TOC entry 4830 (class 2606 OID 69835)
-- Name: employee_tasks employee_tasks_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT employee_tasks_new_pkey PRIMARY KEY (employee_task_id);


--
-- TOC entry 4836 (class 2606 OID 69837)
-- Name: employees employees_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (employee_id);


--
-- TOC entry 4838 (class 2606 OID 69839)
-- Name: favorites favourites_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT favourites_pkey PRIMARY KEY (favorite_id);


--
-- TOC entry 4842 (class 2606 OID 69841)
-- Name: ingredients ingredients_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients
    ADD CONSTRAINT ingredients_pkey PRIMARY KEY (ingredient_id);


--
-- TOC entry 4846 (class 2606 OID 69843)
-- Name: order_items order_items_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT order_items_new_pkey PRIMARY KEY (order_item_id);


--
-- TOC entry 4849 (class 2606 OID 69845)
-- Name: orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (order_id);


--
-- TOC entry 4852 (class 2606 OID 69847)
-- Name: product_batches product_batches_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches
    ADD CONSTRAINT product_batches_new_pkey PRIMARY KEY (product_batch_id);


--
-- TOC entry 4857 (class 2606 OID 69849)
-- Name: product_images product_images_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images
    ADD CONSTRAINT product_images_pkey PRIMARY KEY (product_image_id);


--
-- TOC entry 4854 (class 2606 OID 69851)
-- Name: product_categories production_categories_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_categories
    ADD CONSTRAINT production_categories_pkey PRIMARY KEY (product_category_id);


--
-- TOC entry 4860 (class 2606 OID 69853)
-- Name: products products_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT products_pkey PRIMARY KEY (product_id);


--
-- TOC entry 4864 (class 2606 OID 69855)
-- Name: recipes recipes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT recipes_pkey PRIMARY KEY (recipe_id);


--
-- TOC entry 4866 (class 2606 OID 69857)
-- Name: users system_users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT system_users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4828 (class 2606 OID 69859)
-- Name: companies unique_account_per_company; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT unique_account_per_company UNIQUE (account_id);


--
-- TOC entry 4868 (class 2606 OID 69861)
-- Name: users unique_account_per_user; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT unique_account_per_user UNIQUE (account_id);


--
-- TOC entry 4812 (class 1259 OID 69862)
-- Name: accounts_email_active_unique; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX accounts_email_active_unique ON public.accounts USING btree (((deleted_at IS NULL))) INCLUDE (deleted_at) WITH (deduplicate_items='true');


--
-- TOC entry 4817 (class 1259 OID 69863)
-- Name: fk_cart_items_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_cart_items_account_id_idx ON public.cart_items USING btree (company_id);


--
-- TOC entry 4818 (class 1259 OID 69864)
-- Name: fk_cart_items_product_batch_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_cart_items_product_batch_id_idx ON public.cart_items USING btree (batch_id);


--
-- TOC entry 4831 (class 1259 OID 69865)
-- Name: fk_employee_tasks_employee_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_employee_tasks_employee_id_idx ON public.employee_tasks USING btree (employee_id);


--
-- TOC entry 4832 (class 1259 OID 69866)
-- Name: fk_employee_tasks_order_item_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_employee_tasks_order_item_id_idx ON public.employee_tasks USING btree (order_item_id);


--
-- TOC entry 4839 (class 1259 OID 69867)
-- Name: fk_favorites_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_favorites_account_id_idx ON public.favorites USING btree (company_id);


--
-- TOC entry 4840 (class 1259 OID 69868)
-- Name: fk_favorites_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_favorites_product_id_idx ON public.favorites USING btree (product_id);


--
-- TOC entry 4843 (class 1259 OID 69869)
-- Name: fk_order_items_order_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_order_items_order_id_idx ON public.order_items USING btree (order_id);


--
-- TOC entry 4844 (class 1259 OID 69870)
-- Name: fk_order_items_product_batch_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_order_items_product_batch_id_idx ON public.order_items USING btree (batch_id);


--
-- TOC entry 4847 (class 1259 OID 69871)
-- Name: fk_orders_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_orders_account_id_idx ON public.orders USING btree (company_id);


--
-- TOC entry 4850 (class 1259 OID 69872)
-- Name: fk_product_batches_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_product_batches_product_id_idx ON public.product_batches USING btree (product_id);


--
-- TOC entry 4855 (class 1259 OID 69873)
-- Name: fk_product_images_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_product_images_product_id_idx ON public.product_images USING btree (product_id);


--
-- TOC entry 4858 (class 1259 OID 69874)
-- Name: fk_products_category_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_products_category_id_idx ON public.products USING btree (category_id);


--
-- TOC entry 4861 (class 1259 OID 69875)
-- Name: fk_recipe_ingredient_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_recipe_ingredient_id_idx ON public.recipes USING btree (ingredient_id);


--
-- TOC entry 4862 (class 1259 OID 69876)
-- Name: fk_recipe_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_recipe_product_id_idx ON public.recipes USING btree (product_id);


--
-- TOC entry 4833 (class 1259 OID 69877)
-- Name: idx_employee_tasks_employee_end; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_employee_tasks_employee_end ON public.employee_tasks USING btree (employee_id, end_time);


--
-- TOC entry 4834 (class 1259 OID 69878)
-- Name: idx_employee_tasks_employee_start; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_employee_tasks_employee_start ON public.employee_tasks USING btree (employee_id, start_time);


--
-- TOC entry 4871 (class 2606 OID 69879)
-- Name: companies companies_account_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_account_id_fkey FOREIGN KEY (account_id) REFERENCES public.accounts(account_id) ON DELETE CASCADE;


--
-- TOC entry 4869 (class 2606 OID 69884)
-- Name: cart_items fk_cart_items_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT fk_cart_items_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4870 (class 2606 OID 69889)
-- Name: cart_items fk_cart_items_product_batch_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT fk_cart_items_product_batch_id FOREIGN KEY (batch_id) REFERENCES public.product_batches(product_batch_id);


--
-- TOC entry 4872 (class 2606 OID 69894)
-- Name: employee_tasks fk_employee_tasks_employee_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT fk_employee_tasks_employee_id FOREIGN KEY (employee_id) REFERENCES public.employees(employee_id);


--
-- TOC entry 4873 (class 2606 OID 69899)
-- Name: employee_tasks fk_employee_tasks_order_item_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT fk_employee_tasks_order_item_id FOREIGN KEY (order_item_id) REFERENCES public.order_items(order_item_id);


--
-- TOC entry 4874 (class 2606 OID 69904)
-- Name: favorites fk_favourites_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT fk_favourites_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4875 (class 2606 OID 69909)
-- Name: favorites fk_favourites_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT fk_favourites_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4876 (class 2606 OID 69914)
-- Name: order_items fk_order_items_order_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT fk_order_items_order_id FOREIGN KEY (order_id) REFERENCES public.orders(order_id);


--
-- TOC entry 4877 (class 2606 OID 69919)
-- Name: order_items fk_order_items_product_batch_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT fk_order_items_product_batch_id FOREIGN KEY (batch_id) REFERENCES public.product_batches(product_batch_id);


--
-- TOC entry 4878 (class 2606 OID 69924)
-- Name: orders fk_orders_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT fk_orders_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4879 (class 2606 OID 69929)
-- Name: product_batches fk_product_batches_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches
    ADD CONSTRAINT fk_product_batches_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4880 (class 2606 OID 69934)
-- Name: product_images fk_product_images_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images
    ADD CONSTRAINT fk_product_images_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4881 (class 2606 OID 69939)
-- Name: products fk_products_category_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT fk_products_category_id FOREIGN KEY (category_id) REFERENCES public.product_categories(product_category_id);


--
-- TOC entry 4882 (class 2606 OID 69944)
-- Name: recipes fk_recipe_ingredient_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_recipe_ingredient_id FOREIGN KEY (ingredient_id) REFERENCES public.ingredients(ingredient_id);


--
-- TOC entry 4883 (class 2606 OID 69949)
-- Name: recipes fk_recipe_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_recipe_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4884 (class 2606 OID 69954)
-- Name: users system_users_account_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT system_users_account_id_fkey FOREIGN KEY (account_id) REFERENCES public.accounts(account_id) ON DELETE CASCADE;


-- Completed on 2026-06-28 20:04:07

--
-- PostgreSQL database dump complete
--

\unrestrict kB101P1Ux4eTRIkiCsep3nM51rfrPmOKWdNBnmBrViEFgi5JRqSl6HtmbxT9G2J

