--
-- PostgreSQL database dump
--

\restrict X1EMckF0mq5d4IvwV8pIrLKSqyPByhi9DGxeigtT5XEl6ZZfRqCYj6mimcekCtS

-- Dumped from database version 17.6
-- Dumped by pg_dump version 17.6

-- Started on 2026-01-25 19:02:48

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 925 (class 1247 OID 17144)
-- Name: account_type; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.account_type AS ENUM (
    'user',
    'company'
);


ALTER TYPE public.account_type OWNER TO postgres;

--
-- TOC entry 877 (class 1247 OID 16858)
-- Name: ingredient_batch_status; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.ingredient_batch_status AS ENUM (
    'available',
    'expired',
    'out_of_stock'
);


ALTER TYPE public.ingredient_batch_status OWNER TO postgres;

--
-- TOC entry 886 (class 1247 OID 16882)
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
-- TOC entry 880 (class 1247 OID 16866)
-- Name: order_status; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.order_status AS ENUM (
    'awaiting',
    'in_progress',
    'completed',
    'canceled'
);


ALTER TYPE public.order_status OWNER TO postgres;

--
-- TOC entry 883 (class 1247 OID 16876)
-- Name: user_role; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.user_role AS ENUM (
    'manager_production',
    'admin'
);


ALTER TYPE public.user_role OWNER TO postgres;

--
-- TOC entry 940 (class 1247 OID 17486)
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
-- TOC entry 244 (class 1259 OID 17419)
-- Name: cart_items; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cart_items (
    cart_item_id integer NOT NULL,
    user_id integer NOT NULL,
    batch_id integer,
    quantity integer NOT NULL
);


ALTER TABLE public.cart_items OWNER TO postgres;

--
-- TOC entry 243 (class 1259 OID 17418)
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
-- TOC entry 5065 (class 0 OID 0)
-- Dependencies: 243
-- Name: cart_items_new_cart_item_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cart_items_new_cart_item_id_seq OWNED BY public.cart_items.cart_item_id;


--
-- TOC entry 246 (class 1259 OID 17439)
-- Name: employee_tasks; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.employee_tasks (
    employee_task_id integer NOT NULL,
    employee_id integer NOT NULL,
    order_item_id integer NOT NULL,
    status public.order_status NOT NULL,
    assigned_quantity integer NOT NULL,
    completed_quantity integer DEFAULT 0,
    start_time timestamp with time zone,
    end_time timestamp with time zone,
    CONSTRAINT employee_tasks_new_assigned_quantity_check CHECK ((assigned_quantity > 0)),
    CONSTRAINT employee_tasks_new_completed_quantity_check CHECK ((completed_quantity >= 0))
);


ALTER TABLE public.employee_tasks OWNER TO postgres;

--
-- TOC entry 245 (class 1259 OID 17438)
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
-- TOC entry 5066 (class 0 OID 0)
-- Dependencies: 245
-- Name: employee_tasks_new_employee_task_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employee_tasks_new_employee_task_id_seq OWNED BY public.employee_tasks.employee_task_id;


--
-- TOC entry 218 (class 1259 OID 16904)
-- Name: employees; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.employees (
    employee_id integer NOT NULL,
    firstname character varying(50) NOT NULL,
    lastname character varying(50) NOT NULL,
    patronymic character varying(50),
    birthday date NOT NULL,
    deleted smallint DEFAULT 0 NOT NULL
);


ALTER TABLE public.employees OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 16903)
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
-- TOC entry 5067 (class 0 OID 0)
-- Dependencies: 217
-- Name: employees_employee_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employees_employee_id_seq OWNED BY public.employees.employee_id;


--
-- TOC entry 240 (class 1259 OID 17092)
-- Name: favourites; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.favourites (
    favourite_id integer NOT NULL,
    user_id integer NOT NULL,
    product_id integer NOT NULL
);


ALTER TABLE public.favourites OWNER TO postgres;

--
-- TOC entry 239 (class 1259 OID 17091)
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
-- TOC entry 5068 (class 0 OID 0)
-- Dependencies: 239
-- Name: favourites_favourite_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.favourites_favourite_id_seq OWNED BY public.favourites.favourite_id;


--
-- TOC entry 226 (class 1259 OID 16945)
-- Name: ingredient_batches; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ingredient_batches (
    ingredient_batch_id integer NOT NULL,
    status public.ingredient_batch_status NOT NULL,
    ingredient_id integer NOT NULL,
    purchased_quantity integer NOT NULL,
    remaining_quantity numeric(4,3) NOT NULL,
    delivery_date date NOT NULL,
    expiry_date date NOT NULL
);


ALTER TABLE public.ingredient_batches OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 16944)
-- Name: ingredient_batches_ingredient_batch_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.ingredient_batches_ingredient_batch_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.ingredient_batches_ingredient_batch_id_seq OWNER TO postgres;

--
-- TOC entry 5069 (class 0 OID 0)
-- Dependencies: 225
-- Name: ingredient_batches_ingredient_batch_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredient_batches_ingredient_batch_id_seq OWNED BY public.ingredient_batches.ingredient_batch_id;


--
-- TOC entry 224 (class 1259 OID 16929)
-- Name: ingredients; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ingredients (
    ingredient_id integer NOT NULL,
    supplier_id integer NOT NULL,
    name character varying(100) NOT NULL,
    price numeric(10,2) NOT NULL,
    unit public.ingredient_unit NOT NULL,
    deleted smallint DEFAULT 0 NOT NULL,
    weight numeric(4,3) NOT NULL,
    shelf_life_months integer NOT NULL
);


ALTER TABLE public.ingredients OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 16928)
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
-- TOC entry 5070 (class 0 OID 0)
-- Dependencies: 223
-- Name: ingredients_ingredient_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredients_ingredient_id_seq OWNED BY public.ingredients.ingredient_id;


--
-- TOC entry 242 (class 1259 OID 17399)
-- Name: order_items; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.order_items (
    order_item_id integer NOT NULL,
    order_id integer NOT NULL,
    batch_id integer,
    status public.order_status NOT NULL,
    quantity integer NOT NULL,
    total_units integer
);


ALTER TABLE public.order_items OWNER TO postgres;

--
-- TOC entry 241 (class 1259 OID 17398)
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
-- TOC entry 5071 (class 0 OID 0)
-- Dependencies: 241
-- Name: order_items_new_order_item_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.order_items_new_order_item_id_seq OWNED BY public.order_items.order_item_id;


--
-- TOC entry 234 (class 1259 OID 17009)
-- Name: order_tariffs; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.order_tariffs (
    order_tariff_id integer NOT NULL,
    name character varying(100) NOT NULL,
    description text NOT NULL,
    markup_percent numeric(4,2) NOT NULL,
    free_employees_percent numeric(4,2) NOT NULL,
    deleted smallint DEFAULT 0 NOT NULL
);


ALTER TABLE public.order_tariffs OWNER TO postgres;

--
-- TOC entry 233 (class 1259 OID 17008)
-- Name: order_tariffs_order_tariff_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.order_tariffs_order_tariff_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.order_tariffs_order_tariff_id_seq OWNER TO postgres;

--
-- TOC entry 5072 (class 0 OID 0)
-- Dependencies: 233
-- Name: order_tariffs_order_tariff_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.order_tariffs_order_tariff_id_seq OWNED BY public.order_tariffs.order_tariff_id;


--
-- TOC entry 238 (class 1259 OID 17028)
-- Name: orders; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.orders (
    order_id integer NOT NULL,
    user_id integer NOT NULL,
    tariff_id integer NOT NULL,
    status public.order_status NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL
);


ALTER TABLE public.orders OWNER TO postgres;

--
-- TOC entry 237 (class 1259 OID 17027)
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
-- Dependencies: 237
-- Name: orders_order_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.orders_order_id_seq OWNED BY public.orders.order_id;


--
-- TOC entry 248 (class 1259 OID 17462)
-- Name: product_batches; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product_batches (
    product_batch_id integer NOT NULL,
    product_id integer NOT NULL,
    units integer NOT NULL,
    CONSTRAINT product_batches_new_quantity_check CHECK ((units > 0))
);


ALTER TABLE public.product_batches OWNER TO postgres;

--
-- TOC entry 247 (class 1259 OID 17461)
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
-- Dependencies: 247
-- Name: product_batches_new_product_batch_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.product_batches_new_product_batch_id_seq OWNED BY public.product_batches.product_batch_id;


--
-- TOC entry 220 (class 1259 OID 16912)
-- Name: product_categories; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product_categories (
    product_category_id integer NOT NULL,
    name character varying(100) NOT NULL,
    deleted smallint DEFAULT 0 NOT NULL,
    color character varying(6) NOT NULL,
    icon bytea,
    image bytea
);


ALTER TABLE public.product_categories OWNER TO postgres;

--
-- TOC entry 230 (class 1259 OID 16974)
-- Name: product_images; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product_images (
    product_image_id integer NOT NULL,
    product_id integer NOT NULL,
    image bytea NOT NULL
);


ALTER TABLE public.product_images OWNER TO postgres;

--
-- TOC entry 229 (class 1259 OID 16973)
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
-- Dependencies: 229
-- Name: product_images_product_image_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.product_images_product_image_id_seq OWNED BY public.product_images.product_image_id;


--
-- TOC entry 219 (class 1259 OID 16911)
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
-- Dependencies: 219
-- Name: production_categories_production_category_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.production_categories_production_category_id_seq OWNED BY public.product_categories.product_category_id;


--
-- TOC entry 228 (class 1259 OID 16958)
-- Name: products; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.products (
    product_id integer NOT NULL,
    category_id integer NOT NULL,
    name character varying(100) NOT NULL,
    description text NOT NULL,
    cost_price numeric(4,2) NOT NULL,
    sale_price numeric(4,2) NOT NULL,
    markup_percent integer NOT NULL,
    weight numeric(5,3) NOT NULL,
    production_time integer NOT NULL,
    deleted smallint DEFAULT 0 NOT NULL
);


ALTER TABLE public.products OWNER TO postgres;

--
-- TOC entry 227 (class 1259 OID 16957)
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
-- Dependencies: 227
-- Name: products_product_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.products_product_id_seq OWNED BY public.products.product_id;


--
-- TOC entry 232 (class 1259 OID 16990)
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
-- TOC entry 231 (class 1259 OID 16989)
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
-- Dependencies: 231
-- Name: recipes_recipe_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.recipes_recipe_id_seq OWNED BY public.recipes.recipe_id;


--
-- TOC entry 222 (class 1259 OID 16919)
-- Name: suppliers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.suppliers (
    supplier_id integer NOT NULL,
    name character varying(200) NOT NULL,
    email character varying(255),
    phone character varying(11),
    address text,
    deleted smallint DEFAULT 0
);


ALTER TABLE public.suppliers OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16918)
-- Name: suppliers_supplier_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.suppliers_supplier_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.suppliers_supplier_id_seq OWNER TO postgres;

--
-- TOC entry 5079 (class 0 OID 0)
-- Dependencies: 221
-- Name: suppliers_supplier_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.suppliers_supplier_id_seq OWNED BY public.suppliers.supplier_id;


--
-- TOC entry 236 (class 1259 OID 17018)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    user_id integer NOT NULL,
    role public.user_role NOT NULL,
    firstname character varying(50),
    lastname character varying(50),
    patronymic character varying(50),
    birthday date,
    email character varying(255),
    password character varying(255),
    dismissed smallint DEFAULT 0 NOT NULL,
    account_type public.account_type NOT NULL,
    company_name character varying(150),
    company_inn character varying(12),
    company_ogrn character varying(13),
    company_phone character varying(11),
    company_address text,
    verification_status public.verification_status DEFAULT 'pending'::public.verification_status NOT NULL
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 235 (class 1259 OID 17017)
-- Name: users_user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_user_id_seq OWNER TO postgres;

--
-- TOC entry 5080 (class 0 OID 0)
-- Dependencies: 235
-- Name: users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_user_id_seq OWNED BY public.users.user_id;


--
-- TOC entry 4810 (class 2604 OID 17422)
-- Name: cart_items cart_item_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items ALTER COLUMN cart_item_id SET DEFAULT nextval('public.cart_items_new_cart_item_id_seq'::regclass);


--
-- TOC entry 4811 (class 2604 OID 17442)
-- Name: employee_tasks employee_task_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks ALTER COLUMN employee_task_id SET DEFAULT nextval('public.employee_tasks_new_employee_task_id_seq'::regclass);


--
-- TOC entry 4788 (class 2604 OID 16907)
-- Name: employees employee_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees ALTER COLUMN employee_id SET DEFAULT nextval('public.employees_employee_id_seq'::regclass);


--
-- TOC entry 4808 (class 2604 OID 17095)
-- Name: favourites favourite_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favourites ALTER COLUMN favourite_id SET DEFAULT nextval('public.favourites_favourite_id_seq'::regclass);


--
-- TOC entry 4796 (class 2604 OID 16948)
-- Name: ingredient_batches ingredient_batch_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches ALTER COLUMN ingredient_batch_id SET DEFAULT nextval('public.ingredient_batches_ingredient_batch_id_seq'::regclass);


--
-- TOC entry 4794 (class 2604 OID 16932)
-- Name: ingredients ingredient_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients ALTER COLUMN ingredient_id SET DEFAULT nextval('public.ingredients_ingredient_id_seq'::regclass);


--
-- TOC entry 4809 (class 2604 OID 17402)
-- Name: order_items order_item_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items ALTER COLUMN order_item_id SET DEFAULT nextval('public.order_items_new_order_item_id_seq'::regclass);


--
-- TOC entry 4801 (class 2604 OID 17012)
-- Name: order_tariffs order_tariff_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_tariffs ALTER COLUMN order_tariff_id SET DEFAULT nextval('public.order_tariffs_order_tariff_id_seq'::regclass);


--
-- TOC entry 4806 (class 2604 OID 17031)
-- Name: orders order_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders ALTER COLUMN order_id SET DEFAULT nextval('public.orders_order_id_seq'::regclass);


--
-- TOC entry 4813 (class 2604 OID 17465)
-- Name: product_batches product_batch_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches ALTER COLUMN product_batch_id SET DEFAULT nextval('public.product_batches_new_product_batch_id_seq'::regclass);


--
-- TOC entry 4790 (class 2604 OID 16915)
-- Name: product_categories product_category_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_categories ALTER COLUMN product_category_id SET DEFAULT nextval('public.production_categories_production_category_id_seq'::regclass);


--
-- TOC entry 4799 (class 2604 OID 16977)
-- Name: product_images product_image_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images ALTER COLUMN product_image_id SET DEFAULT nextval('public.product_images_product_image_id_seq'::regclass);


--
-- TOC entry 4797 (class 2604 OID 16961)
-- Name: products product_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products ALTER COLUMN product_id SET DEFAULT nextval('public.products_product_id_seq'::regclass);


--
-- TOC entry 4800 (class 2604 OID 16993)
-- Name: recipes recipe_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes ALTER COLUMN recipe_id SET DEFAULT nextval('public.recipes_recipe_id_seq'::regclass);


--
-- TOC entry 4792 (class 2604 OID 16922)
-- Name: suppliers supplier_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suppliers ALTER COLUMN supplier_id SET DEFAULT nextval('public.suppliers_supplier_id_seq'::regclass);


--
-- TOC entry 4803 (class 2604 OID 17021)
-- Name: users user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.users_user_id_seq'::regclass);


--
-- TOC entry 5055 (class 0 OID 17419)
-- Dependencies: 244
-- Data for Name: cart_items; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.cart_items (cart_item_id, user_id, batch_id, quantity) FROM stdin;
\.


--
-- TOC entry 5057 (class 0 OID 17439)
-- Dependencies: 246
-- Data for Name: employee_tasks; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employee_tasks (employee_task_id, employee_id, order_item_id, status, assigned_quantity, completed_quantity, start_time, end_time) FROM stdin;
\.


--
-- TOC entry 5029 (class 0 OID 16904)
-- Dependencies: 218
-- Data for Name: employees; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employees (employee_id, firstname, lastname, patronymic, birthday, deleted) FROM stdin;
\.


--
-- TOC entry 5051 (class 0 OID 17092)
-- Dependencies: 240
-- Data for Name: favourites; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.favourites (favourite_id, user_id, product_id) FROM stdin;
\.


--
-- TOC entry 5037 (class 0 OID 16945)
-- Dependencies: 226
-- Data for Name: ingredient_batches; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredient_batches (ingredient_batch_id, status, ingredient_id, purchased_quantity, remaining_quantity, delivery_date, expiry_date) FROM stdin;
1	available	1	100	9.500	2025-12-01	2026-12-01
2	available	2	50	8.000	2025-12-15	2026-01-15
3	available	3	50	8.500	2025-11-20	2027-11-20
4	available	4	25	4.800	2025-10-01	2028-10-01
5	available	5	10	1.500	2025-12-10	2026-03-10
6	available	7	20	9.200	2025-12-01	2026-06-01
7	available	8	5	4.900	2025-11-01	2027-05-01
8	available	9	15	4.500	2025-11-15	2026-11-15
9	available	10	2	1.980	2025-10-20	2027-10-20
\.


--
-- TOC entry 5035 (class 0 OID 16929)
-- Dependencies: 224
-- Data for Name: ingredients; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredients (ingredient_id, supplier_id, name, price, unit, deleted, weight, shelf_life_months) FROM stdin;
1	1	Мука пшеничная	45.00	kg	0	1.000	12
2	1	Молоко	65.00	l	0	1.000	1
3	1	Сахар	50.00	kg	0	1.000	24
4	1	Соль	15.00	kg	0	1.000	36
5	1	Дрожжи свежие	120.00	kg	0	0.500	3
6	1	Яйца	90.00	pcs	0	0.060	1
7	1	Масло сливочное	550.00	kg	0	1.000	6
8	1	Корица молотая	350.00	kg	0	0.100	18
9	1	Изюм	280.00	kg	0	1.000	12
10	1	Ванилин	450.00	kg	0	0.050	24
\.


--
-- TOC entry 5053 (class 0 OID 17399)
-- Dependencies: 242
-- Data for Name: order_items; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.order_items (order_item_id, order_id, batch_id, status, quantity, total_units) FROM stdin;
1	1	1	completed	20	20
2	1	2	completed	15	15
3	2	3	completed	25	25
4	2	4	completed	18	18
5	3	1	completed	30	30
6	3	2	completed	22	22
7	4	3	in_progress	15	15
8	4	4	in_progress	10	10
9	5	1	completed	30	30
10	5	2	completed	20	20
11	6	3	completed	20	20
12	6	4	completed	15	15
\.


--
-- TOC entry 5045 (class 0 OID 17009)
-- Dependencies: 234
-- Data for Name: order_tariffs; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.order_tariffs (order_tariff_id, name, description, markup_percent, free_employees_percent, deleted) FROM stdin;
1	Стандартный	Обычный тариф без наценок	0.00	0.00	0
\.


--
-- TOC entry 5049 (class 0 OID 17028)
-- Dependencies: 238
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orders (order_id, user_id, tariff_id, status, start_date, end_date, created_at) FROM stdin;
1	1	1	completed	2025-12-01	2025-12-01	2025-12-01 10:00:00+05
2	1	1	completed	2025-12-05	2025-12-05	2025-12-05 11:30:00+05
3	1	1	completed	2025-12-10	2025-12-10	2025-12-10 14:20:00+05
4	1	1	in_progress	2025-12-15	2025-12-15	2025-12-15 09:15:00+05
5	1	1	completed	2025-11-05	2025-11-05	2025-11-05 12:00:00+05
6	1	1	completed	2025-11-12	2025-11-12	2025-11-12 15:30:00+05
\.


--
-- TOC entry 5059 (class 0 OID 17462)
-- Dependencies: 248
-- Data for Name: product_batches; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_batches (product_batch_id, product_id, units) FROM stdin;
1	1	50
2	2	40
3	1	30
4	2	35
\.


--
-- TOC entry 5031 (class 0 OID 16912)
-- Dependencies: 220
-- Data for Name: product_categories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_categories (product_category_id, name, deleted, color, icon, image) FROM stdin;
1	Булочки	0	FFD700	\N	\N
2	Пироги	0	FF6347	\N	\N
3	Хлеб	0	D2691E	\N	\N
4	Торты	0	FF69B4	\N	\N
5	Печенье	0	F4A460	\N	\N
\.


--
-- TOC entry 5041 (class 0 OID 16974)
-- Dependencies: 230
-- Data for Name: product_images; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_images (product_image_id, product_id, image) FROM stdin;
\.


--
-- TOC entry 5039 (class 0 OID 16958)
-- Dependencies: 228
-- Data for Name: products; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.products (product_id, category_id, name, description, cost_price, sale_price, markup_percent, weight, production_time, deleted) FROM stdin;
1	1	Булочка с корицей	Ароматная сдобная булочка с корицей и сахаром. Идеально подходит к чаю или кофе. Готовится из натуральных ингредиентов.	25.50	55.00	115	0.080	45	0
2	3	Булочка с изюмом	Нежная сдобная булочка с сочным изюмом. Классический рецепт, проверенный временем. Отличный завтрак для всей семьи.	28.00	60.00	114	0.090	50	0
4	5	аывва	аывв	70.00	40.00	25	0.500	15	0
5	5	лощоо	вафыафыв	50.00	60.00	30	0.500	40	0
6	3	adfa	asdfasd	30.00	30.00	120	0.500	50	0
\.


--
-- TOC entry 5043 (class 0 OID 16990)
-- Dependencies: 232
-- Data for Name: recipes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.recipes (recipe_id, product_id, ingredient_id, quantity) FROM stdin;
1	1	1	0.050
2	1	2	0.020
3	1	3	0.010
4	1	4	0.002
5	1	5	0.005
6	1	6	0.500
7	1	7	0.008
8	1	8	0.003
9	2	1	0.055
10	2	2	0.025
11	2	3	0.012
12	2	4	0.002
13	2	5	0.006
14	2	6	0.500
15	2	7	0.010
16	2	9	0.015
17	2	10	0.001
\.


--
-- TOC entry 5033 (class 0 OID 16919)
-- Dependencies: 222
-- Data for Name: suppliers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.suppliers (supplier_id, name, email, phone, address, deleted) FROM stdin;
1	ООО "ПродИнгредиент"	info@prodingredient.ru	79171234567	г. Уфа, ул. Складская, 15	0
\.


--
-- TOC entry 5047 (class 0 OID 17018)
-- Dependencies: 236
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (user_id, role, firstname, lastname, patronymic, birthday, email, password, dismissed, account_type, company_name, company_inn, company_ogrn, company_phone, company_address, verification_status) FROM stdin;
22	admin	asdf	asdf		2020-01-01	asdf	asdf	1	user	\N	\N	\N	\N	\N	pending
2	manager_production	фываф	фаф	Антонович	2006-01-01	fff	fff	0	user	\N	\N	\N	\N	\N	approved
8	admin	ывыфвффы	Ивановsdadad	Илья	2000-01-01	fff	fff	1	user	\N	\N	\N	\N	\N	approved
1	admin	Владлен	Касимов	Ильшатович	2006-01-08	aaa	aaa	0	user	\N	\N	\N	\N	\N	approved
21	admin	Алеша	Шмелев		2006-01-08	asdf	asdf	1	user	\N	\N	\N	\N	\N	pending
\.


--
-- TOC entry 5081 (class 0 OID 0)
-- Dependencies: 243
-- Name: cart_items_new_cart_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.cart_items_new_cart_item_id_seq', 1, false);


--
-- TOC entry 5082 (class 0 OID 0)
-- Dependencies: 245
-- Name: employee_tasks_new_employee_task_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employee_tasks_new_employee_task_id_seq', 1, false);


--
-- TOC entry 5083 (class 0 OID 0)
-- Dependencies: 217
-- Name: employees_employee_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employees_employee_id_seq', 1, false);


--
-- TOC entry 5084 (class 0 OID 0)
-- Dependencies: 239
-- Name: favourites_favourite_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.favourites_favourite_id_seq', 1, false);


--
-- TOC entry 5085 (class 0 OID 0)
-- Dependencies: 225
-- Name: ingredient_batches_ingredient_batch_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredient_batches_ingredient_batch_id_seq', 9, true);


--
-- TOC entry 5086 (class 0 OID 0)
-- Dependencies: 223
-- Name: ingredients_ingredient_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredients_ingredient_id_seq', 10, true);


--
-- TOC entry 5087 (class 0 OID 0)
-- Dependencies: 241
-- Name: order_items_new_order_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.order_items_new_order_item_id_seq', 12, true);


--
-- TOC entry 5088 (class 0 OID 0)
-- Dependencies: 233
-- Name: order_tariffs_order_tariff_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.order_tariffs_order_tariff_id_seq', 1, true);


--
-- TOC entry 5089 (class 0 OID 0)
-- Dependencies: 237
-- Name: orders_order_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orders_order_id_seq', 6, true);


--
-- TOC entry 5090 (class 0 OID 0)
-- Dependencies: 247
-- Name: product_batches_new_product_batch_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.product_batches_new_product_batch_id_seq', 4, true);


--
-- TOC entry 5091 (class 0 OID 0)
-- Dependencies: 229
-- Name: product_images_product_image_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.product_images_product_image_id_seq', 1, true);


--
-- TOC entry 5092 (class 0 OID 0)
-- Dependencies: 219
-- Name: production_categories_production_category_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.production_categories_production_category_id_seq', 5, true);


--
-- TOC entry 5093 (class 0 OID 0)
-- Dependencies: 227
-- Name: products_product_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.products_product_id_seq', 6, true);


--
-- TOC entry 5094 (class 0 OID 0)
-- Dependencies: 231
-- Name: recipes_recipe_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.recipes_recipe_id_seq', 17, true);


--
-- TOC entry 5095 (class 0 OID 0)
-- Dependencies: 221
-- Name: suppliers_supplier_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.suppliers_supplier_id_seq', 1, true);


--
-- TOC entry 5096 (class 0 OID 0)
-- Dependencies: 235
-- Name: users_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_user_id_seq', 22, true);


--
-- TOC entry 4856 (class 2606 OID 17425)
-- Name: cart_items cart_items_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT cart_items_new_pkey PRIMARY KEY (cart_item_id);


--
-- TOC entry 4860 (class 2606 OID 17447)
-- Name: employee_tasks employee_tasks_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT employee_tasks_new_pkey PRIMARY KEY (employee_task_id);


--
-- TOC entry 4818 (class 2606 OID 16910)
-- Name: employees employees_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (employee_id);


--
-- TOC entry 4848 (class 2606 OID 17098)
-- Name: favourites favourites_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favourites
    ADD CONSTRAINT favourites_pkey PRIMARY KEY (favourite_id);


--
-- TOC entry 4828 (class 2606 OID 16950)
-- Name: ingredient_batches ingredient_batches_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches
    ADD CONSTRAINT ingredient_batches_pkey PRIMARY KEY (ingredient_batch_id);


--
-- TOC entry 4825 (class 2606 OID 16937)
-- Name: ingredients ingredients_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients
    ADD CONSTRAINT ingredients_pkey PRIMARY KEY (ingredient_id);


--
-- TOC entry 4854 (class 2606 OID 17405)
-- Name: order_items order_items_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT order_items_new_pkey PRIMARY KEY (order_item_id);


--
-- TOC entry 4840 (class 2606 OID 17016)
-- Name: order_tariffs order_tariffs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_tariffs
    ADD CONSTRAINT order_tariffs_pkey PRIMARY KEY (order_tariff_id);


--
-- TOC entry 4846 (class 2606 OID 17033)
-- Name: orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (order_id);


--
-- TOC entry 4865 (class 2606 OID 17468)
-- Name: product_batches product_batches_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches
    ADD CONSTRAINT product_batches_new_pkey PRIMARY KEY (product_batch_id);


--
-- TOC entry 4834 (class 2606 OID 16982)
-- Name: product_images product_images_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images
    ADD CONSTRAINT product_images_pkey PRIMARY KEY (product_image_id);


--
-- TOC entry 4820 (class 2606 OID 16917)
-- Name: product_categories production_categories_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_categories
    ADD CONSTRAINT production_categories_pkey PRIMARY KEY (product_category_id);


--
-- TOC entry 4831 (class 2606 OID 16966)
-- Name: products products_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT products_pkey PRIMARY KEY (product_id);


--
-- TOC entry 4838 (class 2606 OID 16995)
-- Name: recipes recipes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT recipes_pkey PRIMARY KEY (recipe_id);


--
-- TOC entry 4822 (class 2606 OID 16927)
-- Name: suppliers suppliers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suppliers
    ADD CONSTRAINT suppliers_pkey PRIMARY KEY (supplier_id);


--
-- TOC entry 4842 (class 2606 OID 17026)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4857 (class 1259 OID 17437)
-- Name: fk_cart_items_product_batch_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_cart_items_product_batch_id_idx ON public.cart_items USING btree (batch_id);


--
-- TOC entry 4858 (class 1259 OID 17436)
-- Name: fk_cart_items_user_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_cart_items_user_id_idx ON public.cart_items USING btree (user_id);


--
-- TOC entry 4861 (class 1259 OID 17458)
-- Name: fk_employee_tasks_employee_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_employee_tasks_employee_id_idx ON public.employee_tasks USING btree (employee_id);


--
-- TOC entry 4862 (class 1259 OID 17459)
-- Name: fk_employee_tasks_order_item_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_employee_tasks_order_item_id_idx ON public.employee_tasks USING btree (order_item_id);


--
-- TOC entry 4849 (class 1259 OID 17110)
-- Name: fk_favourites_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_favourites_product_id_idx ON public.favourites USING btree (product_id);


--
-- TOC entry 4850 (class 1259 OID 17109)
-- Name: fk_favourites_user_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_favourites_user_id_idx ON public.favourites USING btree (user_id);


--
-- TOC entry 4826 (class 1259 OID 16956)
-- Name: fk_ingredient_batches_ingredient_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_ingredient_batches_ingredient_id_idx ON public.ingredient_batches USING btree (ingredient_id);


--
-- TOC entry 4823 (class 1259 OID 16943)
-- Name: fk_ingredients_supplier_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_ingredients_supplier_id_idx ON public.ingredients USING btree (supplier_id);


--
-- TOC entry 4851 (class 1259 OID 17416)
-- Name: fk_order_items_order_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_order_items_order_id_idx ON public.order_items USING btree (order_id);


--
-- TOC entry 4852 (class 1259 OID 17417)
-- Name: fk_order_items_product_batch_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_order_items_product_batch_id_idx ON public.order_items USING btree (batch_id);


--
-- TOC entry 4843 (class 1259 OID 17050)
-- Name: fk_orders_tariff_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_orders_tariff_id_idx ON public.orders USING btree (tariff_id);


--
-- TOC entry 4844 (class 1259 OID 17051)
-- Name: fk_orders_user_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_orders_user_id_idx ON public.orders USING btree (user_id);


--
-- TOC entry 4863 (class 1259 OID 17474)
-- Name: fk_product_batches_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_product_batches_product_id_idx ON public.product_batches USING btree (product_id);


--
-- TOC entry 4832 (class 1259 OID 16988)
-- Name: fk_product_images_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_product_images_product_id_idx ON public.product_images USING btree (product_id);


--
-- TOC entry 4829 (class 1259 OID 16972)
-- Name: fk_products_category_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_products_category_id_idx ON public.products USING btree (category_id);


--
-- TOC entry 4835 (class 1259 OID 17007)
-- Name: fk_recipe_ingredient_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_recipe_ingredient_id_idx ON public.recipes USING btree (ingredient_id);


--
-- TOC entry 4836 (class 1259 OID 17006)
-- Name: fk_recipe_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_recipe_product_id_idx ON public.recipes USING btree (product_id);


--
-- TOC entry 4878 (class 2606 OID 17475)
-- Name: cart_items fk_cart_items_product_batch_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT fk_cart_items_product_batch_id FOREIGN KEY (batch_id) REFERENCES public.product_batches(product_batch_id);


--
-- TOC entry 4879 (class 2606 OID 17426)
-- Name: cart_items fk_cart_items_user_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT fk_cart_items_user_id FOREIGN KEY (user_id) REFERENCES public.users(user_id);


--
-- TOC entry 4880 (class 2606 OID 17448)
-- Name: employee_tasks fk_employee_tasks_employee_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT fk_employee_tasks_employee_id FOREIGN KEY (employee_id) REFERENCES public.employees(employee_id);


--
-- TOC entry 4881 (class 2606 OID 17453)
-- Name: employee_tasks fk_employee_tasks_order_item_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT fk_employee_tasks_order_item_id FOREIGN KEY (order_item_id) REFERENCES public.order_items(order_item_id);


--
-- TOC entry 4874 (class 2606 OID 17104)
-- Name: favourites fk_favourites_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favourites
    ADD CONSTRAINT fk_favourites_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4875 (class 2606 OID 17138)
-- Name: favourites fk_favourites_user_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favourites
    ADD CONSTRAINT fk_favourites_user_id FOREIGN KEY (user_id) REFERENCES public.users(user_id);


--
-- TOC entry 4867 (class 2606 OID 16951)
-- Name: ingredient_batches fk_ingredient_batches_ingredient_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches
    ADD CONSTRAINT fk_ingredient_batches_ingredient_id FOREIGN KEY (ingredient_id) REFERENCES public.ingredients(ingredient_id);


--
-- TOC entry 4866 (class 2606 OID 16938)
-- Name: ingredients fk_ingredients_supplier_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients
    ADD CONSTRAINT fk_ingredients_supplier_id FOREIGN KEY (supplier_id) REFERENCES public.suppliers(supplier_id);


--
-- TOC entry 4876 (class 2606 OID 17406)
-- Name: order_items fk_order_items_order_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT fk_order_items_order_id FOREIGN KEY (order_id) REFERENCES public.orders(order_id);


--
-- TOC entry 4877 (class 2606 OID 17480)
-- Name: order_items fk_order_items_product_batch_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT fk_order_items_product_batch_id FOREIGN KEY (batch_id) REFERENCES public.product_batches(product_batch_id);


--
-- TOC entry 4872 (class 2606 OID 17039)
-- Name: orders fk_orders_tariff_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT fk_orders_tariff_id FOREIGN KEY (tariff_id) REFERENCES public.order_tariffs(order_tariff_id);


--
-- TOC entry 4873 (class 2606 OID 17044)
-- Name: orders fk_orders_user_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT fk_orders_user_id FOREIGN KEY (user_id) REFERENCES public.users(user_id);


--
-- TOC entry 4882 (class 2606 OID 17469)
-- Name: product_batches fk_product_batches_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches
    ADD CONSTRAINT fk_product_batches_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4869 (class 2606 OID 16983)
-- Name: product_images fk_product_images_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images
    ADD CONSTRAINT fk_product_images_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4868 (class 2606 OID 16967)
-- Name: products fk_products_category_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT fk_products_category_id FOREIGN KEY (category_id) REFERENCES public.product_categories(product_category_id);


--
-- TOC entry 4870 (class 2606 OID 17001)
-- Name: recipes fk_recipe_ingredient_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_recipe_ingredient_id FOREIGN KEY (ingredient_id) REFERENCES public.ingredients(ingredient_id);


--
-- TOC entry 4871 (class 2606 OID 16996)
-- Name: recipes fk_recipe_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_recipe_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


-- Completed on 2026-01-25 19:02:49

--
-- PostgreSQL database dump complete
--

\unrestrict X1EMckF0mq5d4IvwV8pIrLKSqyPByhi9DGxeigtT5XEl6ZZfRqCYj6mimcekCtS

