--
-- PostgreSQL database dump
--

\restrict 5JjS6p4540QkWPz4f3cYUzNGDkZKnnbMdN1CRtkMe4DVVXLggQwoCRgTuJAitQx

-- Dumped from database version 17.6
-- Dumped by pg_dump version 17.6

-- Started on 2026-05-22 04:35:49

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
-- TOC entry 5 (class 2615 OID 61002)
-- Name: public; Type: SCHEMA; Schema: -; Owner: postgres
--

-- *not* creating schema, since initdb creates it


ALTER SCHEMA public OWNER TO postgres;

--
-- TOC entry 5115 (class 0 OID 0)
-- Dependencies: 5
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: postgres
--

COMMENT ON SCHEMA public IS '';


--
-- TOC entry 883 (class 1247 OID 61004)
-- Name: account_type; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.account_type AS ENUM (
    'user',
    'company'
);


ALTER TYPE public.account_type OWNER TO postgres;

--
-- TOC entry 886 (class 1247 OID 61010)
-- Name: ingredient_batch_status; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.ingredient_batch_status AS ENUM (
    'available',
    'expired',
    'out_of_stock',
    'archived'
);


ALTER TYPE public.ingredient_batch_status OWNER TO postgres;

--
-- TOC entry 889 (class 1247 OID 61018)
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
-- TOC entry 955 (class 1247 OID 61615)
-- Name: order_status; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.order_status AS ENUM (
    'created',
    'in_progress',
    'completed',
    'canceled'
);


ALTER TYPE public.order_status OWNER TO postgres;

--
-- TOC entry 892 (class 1247 OID 61040)
-- Name: user_role; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.user_role AS ENUM (
    'commercial_manager',
    'technologist'
);


ALTER TYPE public.user_role OWNER TO postgres;

--
-- TOC entry 895 (class 1247 OID 61046)
-- Name: verification_status; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.verification_status AS ENUM (
    'pending',
    'approved',
    'rejected',
    'suspended',
    'archived'
);


ALTER TYPE public.verification_status OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 217 (class 1259 OID 61055)
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
-- TOC entry 218 (class 1259 OID 61062)
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
-- TOC entry 5117 (class 0 OID 0)
-- Dependencies: 218
-- Name: accounts_account_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.accounts_account_id_seq OWNED BY public.accounts.account_id;


--
-- TOC entry 219 (class 1259 OID 61063)
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
-- TOC entry 220 (class 1259 OID 61066)
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
-- TOC entry 5118 (class 0 OID 0)
-- Dependencies: 220
-- Name: cart_items_new_cart_item_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cart_items_new_cart_item_id_seq OWNED BY public.cart_items.cart_item_id;


--
-- TOC entry 221 (class 1259 OID 61067)
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
-- TOC entry 222 (class 1259 OID 61072)
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
-- TOC entry 5119 (class 0 OID 0)
-- Dependencies: 222
-- Name: companies_company_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.companies_company_id_seq OWNED BY public.companies.company_id;


--
-- TOC entry 223 (class 1259 OID 61073)
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
    status public.order_status,
    CONSTRAINT employee_tasks_new_assigned_quantity_check CHECK ((assigned_quantity > 0)),
    CONSTRAINT employee_tasks_new_completed_quantity_check CHECK ((completed_quantity >= 0))
);


ALTER TABLE public.employee_tasks OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 61079)
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
-- TOC entry 5120 (class 0 OID 0)
-- Dependencies: 224
-- Name: employee_tasks_new_employee_task_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employee_tasks_new_employee_task_id_seq OWNED BY public.employee_tasks.employee_task_id;


--
-- TOC entry 225 (class 1259 OID 61080)
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
-- TOC entry 226 (class 1259 OID 61083)
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
-- TOC entry 5121 (class 0 OID 0)
-- Dependencies: 226
-- Name: employees_employee_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employees_employee_id_seq OWNED BY public.employees.employee_id;


--
-- TOC entry 227 (class 1259 OID 61084)
-- Name: favorites; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.favorites (
    favorite_id integer NOT NULL,
    product_id integer NOT NULL,
    company_id integer
);


ALTER TABLE public.favorites OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 61087)
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
-- TOC entry 5122 (class 0 OID 0)
-- Dependencies: 228
-- Name: favourites_favourite_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.favourites_favourite_id_seq OWNED BY public.favorites.favorite_id;


--
-- TOC entry 229 (class 1259 OID 61088)
-- Name: ingredient_batches; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ingredient_batches (
    ingredient_batch_id integer NOT NULL,
    status public.ingredient_batch_status NOT NULL,
    purchased_quantity integer NOT NULL,
    remaining_quantity numeric(10,3) NOT NULL,
    delivery_date date NOT NULL,
    expiry_date date NOT NULL,
    supplier_ingredient_id integer NOT NULL
);


ALTER TABLE public.ingredient_batches OWNER TO postgres;

--
-- TOC entry 230 (class 1259 OID 61091)
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
-- TOC entry 5123 (class 0 OID 0)
-- Dependencies: 230
-- Name: ingredient_batches_ingredient_batch_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredient_batches_ingredient_batch_id_seq OWNED BY public.ingredient_batches.ingredient_batch_id;


--
-- TOC entry 231 (class 1259 OID 61092)
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
-- TOC entry 232 (class 1259 OID 61095)
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
-- TOC entry 5124 (class 0 OID 0)
-- Dependencies: 232
-- Name: ingredients_ingredient_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredients_ingredient_id_seq OWNED BY public.ingredients.ingredient_id;


--
-- TOC entry 254 (class 1259 OID 61352)
-- Name: order_item_ingredient_reservations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.order_item_ingredient_reservations (
    order_item_ingredient_reservation_id integer NOT NULL,
    order_item_id integer NOT NULL,
    ingredient_batch_id integer NOT NULL,
    reserved_quantity numeric(18,4) DEFAULT 0 NOT NULL,
    reserved_unit character varying(10) DEFAULT 'G'::character varying NOT NULL
);


ALTER TABLE public.order_item_ingredient_reservations OWNER TO postgres;

--
-- TOC entry 253 (class 1259 OID 61351)
-- Name: order_item_ingredient_reserva_order_item_ingredient_reserva_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.order_item_ingredient_reserva_order_item_ingredient_reserva_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.order_item_ingredient_reserva_order_item_ingredient_reserva_seq OWNER TO postgres;

--
-- TOC entry 5125 (class 0 OID 0)
-- Dependencies: 253
-- Name: order_item_ingredient_reserva_order_item_ingredient_reserva_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.order_item_ingredient_reserva_order_item_ingredient_reserva_seq OWNED BY public.order_item_ingredient_reservations.order_item_ingredient_reservation_id;


--
-- TOC entry 233 (class 1259 OID 61096)
-- Name: order_items; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.order_items (
    order_item_id integer NOT NULL,
    order_id integer NOT NULL,
    batch_id integer NOT NULL,
    quantity integer NOT NULL,
    unit_price numeric(10,2) DEFAULT 0 NOT NULL,
    units_per_batch integer NOT NULL,
    status public.order_status
);


ALTER TABLE public.order_items OWNER TO postgres;

--
-- TOC entry 234 (class 1259 OID 61100)
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
-- TOC entry 5126 (class 0 OID 0)
-- Dependencies: 234
-- Name: order_items_new_order_item_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.order_items_new_order_item_id_seq OWNED BY public.order_items.order_item_id;


--
-- TOC entry 235 (class 1259 OID 61101)
-- Name: orders; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.orders (
    order_id integer NOT NULL,
    start_date date,
    end_date date NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    company_id integer NOT NULL,
    canceled_at timestamp with time zone,
    cancel_reason text,
    status public.order_status
);


ALTER TABLE public.orders OWNER TO postgres;

--
-- TOC entry 236 (class 1259 OID 61107)
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
-- TOC entry 5127 (class 0 OID 0)
-- Dependencies: 236
-- Name: orders_order_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.orders_order_id_seq OWNED BY public.orders.order_id;


--
-- TOC entry 237 (class 1259 OID 61108)
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
-- TOC entry 238 (class 1259 OID 61113)
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
-- TOC entry 5128 (class 0 OID 0)
-- Dependencies: 238
-- Name: product_batches_new_product_batch_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.product_batches_new_product_batch_id_seq OWNED BY public.product_batches.product_batch_id;


--
-- TOC entry 239 (class 1259 OID 61114)
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
-- TOC entry 240 (class 1259 OID 61117)
-- Name: product_images; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product_images (
    product_image_id integer NOT NULL,
    product_id integer NOT NULL,
    image_path character varying
);


ALTER TABLE public.product_images OWNER TO postgres;

--
-- TOC entry 241 (class 1259 OID 61122)
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
-- TOC entry 5129 (class 0 OID 0)
-- Dependencies: 241
-- Name: product_images_product_image_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.product_images_product_image_id_seq OWNED BY public.product_images.product_image_id;


--
-- TOC entry 242 (class 1259 OID 61123)
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
-- TOC entry 5130 (class 0 OID 0)
-- Dependencies: 242
-- Name: production_categories_production_category_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.production_categories_production_category_id_seq OWNED BY public.product_categories.product_category_id;


--
-- TOC entry 243 (class 1259 OID 61124)
-- Name: products; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.products (
    product_id integer NOT NULL,
    category_id integer NOT NULL,
    name character varying(100) NOT NULL,
    description text NOT NULL,
    cost_price numeric(10,2) NOT NULL,
    weight numeric(5,3) NOT NULL,
    production_time_minutes integer NOT NULL,
    deleted_at timestamp with time zone,
    storage_temp_min numeric(4,1) DEFAULT 2.0,
    storage_temp_max numeric(4,1) DEFAULT 6.0,
    shelf_life_days integer DEFAULT 3
);


ALTER TABLE public.products OWNER TO postgres;

--
-- TOC entry 244 (class 1259 OID 61132)
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
-- TOC entry 5131 (class 0 OID 0)
-- Dependencies: 244
-- Name: products_product_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.products_product_id_seq OWNED BY public.products.product_id;


--
-- TOC entry 245 (class 1259 OID 61133)
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
-- TOC entry 246 (class 1259 OID 61136)
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
-- TOC entry 5132 (class 0 OID 0)
-- Dependencies: 246
-- Name: recipes_recipe_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.recipes_recipe_id_seq OWNED BY public.recipes.recipe_id;


--
-- TOC entry 247 (class 1259 OID 61137)
-- Name: supplier_ingredients; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.supplier_ingredients (
    supplier_ingredient_id integer NOT NULL,
    supplier_id integer NOT NULL,
    ingredient_id integer NOT NULL,
    name character varying(100) NOT NULL,
    price numeric(10,2) NOT NULL,
    weight numeric(10,6) NOT NULL,
    shelf_life_days integer NOT NULL,
    deleted_at timestamp with time zone,
    unit public.ingredient_unit NOT NULL
);


ALTER TABLE public.supplier_ingredients OWNER TO postgres;

--
-- TOC entry 248 (class 1259 OID 61140)
-- Name: supplier_ingredient_supplier_ingredient_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.supplier_ingredient_supplier_ingredient_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.supplier_ingredient_supplier_ingredient_id_seq OWNER TO postgres;

--
-- TOC entry 5133 (class 0 OID 0)
-- Dependencies: 248
-- Name: supplier_ingredient_supplier_ingredient_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.supplier_ingredient_supplier_ingredient_id_seq OWNED BY public.supplier_ingredients.supplier_ingredient_id;


--
-- TOC entry 249 (class 1259 OID 61141)
-- Name: suppliers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.suppliers (
    supplier_id integer NOT NULL,
    name character varying(200) NOT NULL,
    email character varying(255),
    phone character varying(11),
    address text,
    deleted_at timestamp with time zone
);


ALTER TABLE public.suppliers OWNER TO postgres;

--
-- TOC entry 250 (class 1259 OID 61146)
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
-- TOC entry 5134 (class 0 OID 0)
-- Dependencies: 250
-- Name: suppliers_supplier_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.suppliers_supplier_id_seq OWNED BY public.suppliers.supplier_id;


--
-- TOC entry 251 (class 1259 OID 61147)
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
-- TOC entry 252 (class 1259 OID 61150)
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
-- TOC entry 5135 (class 0 OID 0)
-- Dependencies: 252
-- Name: system_users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.system_users_user_id_seq OWNED BY public.users.user_id;


--
-- TOC entry 4803 (class 2604 OID 61151)
-- Name: accounts account_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts ALTER COLUMN account_id SET DEFAULT nextval('public.accounts_account_id_seq'::regclass);


--
-- TOC entry 4806 (class 2604 OID 61152)
-- Name: cart_items cart_item_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items ALTER COLUMN cart_item_id SET DEFAULT nextval('public.cart_items_new_cart_item_id_seq'::regclass);


--
-- TOC entry 4807 (class 2604 OID 61153)
-- Name: companies company_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies ALTER COLUMN company_id SET DEFAULT nextval('public.companies_company_id_seq'::regclass);


--
-- TOC entry 4808 (class 2604 OID 61154)
-- Name: employee_tasks employee_task_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks ALTER COLUMN employee_task_id SET DEFAULT nextval('public.employee_tasks_new_employee_task_id_seq'::regclass);


--
-- TOC entry 4810 (class 2604 OID 61155)
-- Name: employees employee_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees ALTER COLUMN employee_id SET DEFAULT nextval('public.employees_employee_id_seq'::regclass);


--
-- TOC entry 4811 (class 2604 OID 61156)
-- Name: favorites favorite_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites ALTER COLUMN favorite_id SET DEFAULT nextval('public.favourites_favourite_id_seq'::regclass);


--
-- TOC entry 4812 (class 2604 OID 61157)
-- Name: ingredient_batches ingredient_batch_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches ALTER COLUMN ingredient_batch_id SET DEFAULT nextval('public.ingredient_batches_ingredient_batch_id_seq'::regclass);


--
-- TOC entry 4813 (class 2604 OID 61158)
-- Name: ingredients ingredient_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients ALTER COLUMN ingredient_id SET DEFAULT nextval('public.ingredients_ingredient_id_seq'::regclass);


--
-- TOC entry 4830 (class 2604 OID 61355)
-- Name: order_item_ingredient_reservations order_item_ingredient_reservation_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_item_ingredient_reservations ALTER COLUMN order_item_ingredient_reservation_id SET DEFAULT nextval('public.order_item_ingredient_reserva_order_item_ingredient_reserva_seq'::regclass);


--
-- TOC entry 4814 (class 2604 OID 61159)
-- Name: order_items order_item_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items ALTER COLUMN order_item_id SET DEFAULT nextval('public.order_items_new_order_item_id_seq'::regclass);


--
-- TOC entry 4816 (class 2604 OID 61160)
-- Name: orders order_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders ALTER COLUMN order_id SET DEFAULT nextval('public.orders_order_id_seq'::regclass);


--
-- TOC entry 4818 (class 2604 OID 61161)
-- Name: product_batches product_batch_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches ALTER COLUMN product_batch_id SET DEFAULT nextval('public.product_batches_new_product_batch_id_seq'::regclass);


--
-- TOC entry 4820 (class 2604 OID 61162)
-- Name: product_categories product_category_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_categories ALTER COLUMN product_category_id SET DEFAULT nextval('public.production_categories_production_category_id_seq'::regclass);


--
-- TOC entry 4821 (class 2604 OID 61163)
-- Name: product_images product_image_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images ALTER COLUMN product_image_id SET DEFAULT nextval('public.product_images_product_image_id_seq'::regclass);


--
-- TOC entry 4822 (class 2604 OID 61164)
-- Name: products product_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products ALTER COLUMN product_id SET DEFAULT nextval('public.products_product_id_seq'::regclass);


--
-- TOC entry 4826 (class 2604 OID 61165)
-- Name: recipes recipe_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes ALTER COLUMN recipe_id SET DEFAULT nextval('public.recipes_recipe_id_seq'::regclass);


--
-- TOC entry 4827 (class 2604 OID 61166)
-- Name: supplier_ingredients supplier_ingredient_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supplier_ingredients ALTER COLUMN supplier_ingredient_id SET DEFAULT nextval('public.supplier_ingredient_supplier_ingredient_id_seq'::regclass);


--
-- TOC entry 4828 (class 2604 OID 61167)
-- Name: suppliers supplier_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suppliers ALTER COLUMN supplier_id SET DEFAULT nextval('public.suppliers_supplier_id_seq'::regclass);


--
-- TOC entry 4829 (class 2604 OID 61168)
-- Name: users user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.system_users_user_id_seq'::regclass);


--
-- TOC entry 5072 (class 0 OID 61055)
-- Dependencies: 217
-- Data for Name: accounts; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.accounts (account_id, email, password_hash, account_type, verification_status, session, session_expires_at, deleted_at, created_at) FROM stdin;
6	adgf@afda.ru	$2a$11$YxT7xVoWxaqFSkcBqLvCFOxyX3ozJ43CtYA1c2vlj/7wcYc49aW9e	company	approved	ffcf57284a6045fbb6d0b12482600bfc	2026-04-15 04:52:52.89745+05	\N	2026-04-14 04:52:41.668601+05
11	vlad@prem.mud	$2a$11$9cvydb5jE137PaSxwtr.X.o0b9kFM7KHk8W946FeiOPr24hGmfDiC	user	suspended	\N	\N	\N	2026-05-06 05:22:32.066096+05
3	aaa@gmail.com	$2a$11$wvUOiHFVrpnNSJxd8/0KpOQsD9S8bM4iuD.3t4tqOexJzcXuMUUtC	user	rejected	\N	\N	2026-05-08 18:57:27.593172+05	2026-04-01 00:00:00+05
14	bulochka@gmail.com	$2a$11$62yhhUaQr673.iyJO.ZGeOWrihupD0f5rXg5aSBlExhMgmfxKMZR.	company	pending	\N	\N	2026-05-08 18:57:37.060627+05	2026-05-08 02:27:21.654305+05
15	mmm@obman.ru	$2a$11$hJgCLcbp4loIiGX/gDESX.9MWTKF/u6R50V/Rl..Rek78Q5RBn46W	company	approved	5a3fcd42722244c8bba74988ecd0be62	2026-05-23 03:43:05.805688+05	\N	2026-05-10 18:43:56.359528+05
13	asdfsdf@ga.ru	$2a$11$k07rbeW5ANGlV5045X46ludGdBeuuOXh2rEvKE/kvjApYSc1B1AyG	user	approved	\N	\N	\N	2026-05-06 19:34:24.611444+05
2	tech@bakery.ru	$2a$11$0VQvxvz2C5uNESDnKmzsw.NYh0bZa6Tk9lKCP4f/egg1qMGG.M0h.	user	approved	32fe609f85cc4744988e4a692c140084	2026-05-23 04:32:53.234015+05	\N	2026-04-01 00:00:00+05
1	adfasdf@df.tae	$2a$11$o63Eud8w76K/tKwLNIv6cuV6ElGrL3z4VXOPNNK64ZtEMAicVrsu6	company	approved	\N	\N	\N	2026-04-01 00:00:00+05
9	kas@gmail.com	$2a$11$VtUqilLFjagj6BViMrRk0eCKe7GCFUyE/harMBg5U3AmWv80RpzVq	user	pending	\N	\N	\N	2026-05-06 05:15:43.713489+05
10	olga@mail.ru	$2a$11$Gd.2De.Ow.Z.NL2vnrvG4eam7ivQGkhNgPi9HahxVKqNceZlOwy/S	user	pending	\N	\N	\N	2026-05-06 05:16:27.091247+05
12	sdfas@adff.r	$2a$11$UMu4rI5ZnQAafeFw2erzb.ckD1MDylVYTBJyiqgnm/MPuL24hhJha	user	pending	\N	\N	2026-05-06 05:31:50.174239+05	2026-05-06 05:31:40.918055+05
\.


--
-- TOC entry 5074 (class 0 OID 61063)
-- Dependencies: 219
-- Data for Name: cart_items; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.cart_items (cart_item_id, batch_id, quantity, company_id) FROM stdin;
23	1	4	6
21	24	4	6
35	27	66	1
\.


--
-- TOC entry 5076 (class 0 OID 61067)
-- Dependencies: 221
-- Data for Name: companies; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.companies (company_id, account_id, name, inn, ogrn, phone, address) FROM stdin;
4	6	agsdfg	1341253145	1543532534535	\N	\N
5	14	ООО Булочка	5349593453	1234523952354	\N	\N
1	1	Сикс севен	5793851133	5264386528962	89432454345	Город Уфа, улица 50 лет СССР, дом 1, главный офис на торце дома
6	15	МММ	1543142354	5451342523452	\N	\N
\.


--
-- TOC entry 5078 (class 0 OID 61073)
-- Dependencies: 223
-- Data for Name: employee_tasks; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employee_tasks (employee_task_id, employee_id, order_item_id, assigned_quantity, completed_quantity, start_time, end_time, status) FROM stdin;
1	1	2	1	0	2026-04-02 16:00:00+05	2026-04-02 16:30:00+05	completed
2	2	2	1	0	2026-04-02 16:00:00+05	2026-04-02 16:30:00+05	completed
3	1	1	1	0	2026-04-02 16:30:00+05	2026-04-02 17:00:00+05	completed
4	2	1	1	0	2026-04-02 16:30:00+05	2026-04-02 17:00:00+05	completed
11	1	12	2	0	2026-05-26 15:10:00+05	2026-05-26 17:00:00+05	completed
12	2	12	2	0	2026-05-26 15:10:00+05	2026-05-26 17:00:00+05	completed
13	11	12	1	0	2026-05-26 16:05:00+05	2026-05-26 17:00:00+05	completed
8	1	11	2	0	2026-06-19 13:20:00+05	2026-06-19 15:10:00+05	completed
9	2	11	2	0	2026-06-19 13:20:00+05	2026-06-19 15:10:00+05	completed
10	11	11	1	0	2026-06-19 15:10:00+05	2026-06-19 16:05:00+05	completed
14	2	13	2	0	2026-05-22 15:10:00+05	2026-05-22 17:00:00+05	created
15	11	13	3	0	2026-05-22 14:15:00+05	2026-05-22 17:00:00+05	created
16	2	14	1	0	2026-05-22 11:05:00+05	2026-05-22 12:00:00+05	created
17	2	14	2	0	2026-05-22 13:20:00+05	2026-05-22 15:10:00+05	created
18	11	14	1	0	2026-05-22 11:05:00+05	2026-05-22 12:00:00+05	created
19	11	14	1	0	2026-05-22 13:20:00+05	2026-05-22 14:15:00+05	created
20	2	14	2	0	2026-05-22 09:15:00+05	2026-05-22 11:05:00+05	created
21	11	14	3	0	2026-05-22 08:20:00+05	2026-05-22 11:05:00+05	created
22	2	16	3	0	2026-05-25 14:15:00+05	2026-05-25 17:00:00+05	canceled
23	11	16	2	0	2026-05-25 15:10:00+05	2026-05-25 17:00:00+05	canceled
24	2	16	1	0	2026-05-25 11:05:00+05	2026-05-25 12:00:00+05	canceled
25	2	16	1	0	2026-05-25 13:20:00+05	2026-05-25 14:15:00+05	canceled
26	11	16	1	0	2026-05-25 11:05:00+05	2026-05-25 12:00:00+05	canceled
27	11	16	2	0	2026-05-25 13:20:00+05	2026-05-25 15:10:00+05	canceled
28	2	17	2	0	2026-06-18 10:10:00+05	2026-06-18 12:00:00+05	created
29	2	17	4	0	2026-06-18 13:20:00+05	2026-06-18 17:00:00+05	created
30	2	17	4	0	2026-06-19 08:20:00+05	2026-06-19 12:00:00+05	created
31	2	17	2	0	2026-06-19 15:10:00+05	2026-06-19 17:00:00+05	created
32	11	17	2	0	2026-06-18 10:10:00+05	2026-06-18 12:00:00+05	created
33	11	17	4	0	2026-06-18 13:20:00+05	2026-06-18 17:00:00+05	created
34	11	17	4	0	2026-06-19 08:20:00+05	2026-06-19 12:00:00+05	created
35	11	17	2	0	2026-06-19 13:20:00+05	2026-06-19 15:10:00+05	created
36	11	17	1	0	2026-06-19 16:05:00+05	2026-06-19 17:00:00+05	created
37	2	22	2	0	2026-06-30 15:10:00+05	2026-06-30 17:00:00+05	created
38	11	22	3	0	2026-06-30 14:15:00+05	2026-06-30 17:00:00+05	created
\.


--
-- TOC entry 5080 (class 0 OID 61080)
-- Dependencies: 225
-- Data for Name: employees; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employees (employee_id, firstname, lastname, patronymic, birthday, deleted_at) FROM stdin;
2	Петр	Ян	Алексеевич	1987-05-20	\N
5	Ишмурзин	Никита	\N	1971-07-01	2026-05-08 18:57:46.842718+05
10	Влад	Бумага	\N	1991-01-01	2026-05-20 06:58:24.710371+05
9	Владимир	Путин	\N	1987-01-01	2026-05-20 06:58:26.36671+05
8	Олег	Варежков	\N	1994-01-01	2026-05-20 06:58:27.79103+05
7	Иван	Золо	\N	1994-01-01	2026-05-20 06:58:29.462353+05
6	Дмитрий	Нагиев	\N	1971-07-01	2026-05-20 06:58:31.007312+05
4	Олег	Тинькофф	\N	1980-01-01	2026-05-20 06:58:32.631325+05
3	Алексей	Навальный	\N	1979-01-01	2026-05-20 06:58:34.704288+05
11	Алексей	Навальный	\N	1964-01-01	\N
1	Владлен	Касимов		1985-07-10	2026-05-21 15:09:03.63785+05
\.


--
-- TOC entry 5082 (class 0 OID 61084)
-- Dependencies: 227
-- Data for Name: favorites; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.favorites (favorite_id, product_id, company_id) FROM stdin;
2	1	1
3	2	1
\.


--
-- TOC entry 5084 (class 0 OID 61088)
-- Dependencies: 229
-- Data for Name: ingredient_batches; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredient_batches (ingredient_batch_id, status, purchased_quantity, remaining_quantity, delivery_date, expiry_date, supplier_ingredient_id) FROM stdin;
1	available	100	85.500	2026-03-25	2026-09-25	1
4	available	200	180.000	2026-04-01	2026-05-01	4
2	available	50	37.000	2026-03-28	2026-12-28	2
3	available	30	25.495	2026-03-30	2026-06-30	3
\.


--
-- TOC entry 5086 (class 0 OID 61092)
-- Dependencies: 231
-- Data for Name: ingredients; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredients (ingredient_id, name, deleted_at, base_unit) FROM stdin;
3	Масло сливочное	\N	kg
6	Соль обычная	2026-05-12 23:50:34.544599+05	g
15	Яблоки	\N	g
16	Вишня	2026-05-13 00:00:23.756121+05	g
17	Мандарин	\N	g
18	Виноград	\N	pcs
1	Мука	\N	kg
2	Сахар-песок	\N	g
5	Молоко  3,2%	\N	l
19	Сахар белый	2026-05-22 01:22:07.564893+05	kg
20	Сахар-песок	2026-05-22 01:22:20.422271+05	kg
4	Яйца куриные	\N	g
21	Сахар	2026-05-22 03:39:38.283206+05	g
\.


--
-- TOC entry 5109 (class 0 OID 61352)
-- Dependencies: 254
-- Data for Name: order_item_ingredient_reservations; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.order_item_ingredient_reservations (order_item_ingredient_reservation_id, order_item_id, ingredient_batch_id, reserved_quantity, reserved_unit) FROM stdin;
5	11	3	0.0050	g
6	11	2	5.0000	g
7	12	3	0.0050	G
8	12	2	5.0000	G
9	13	3	0.0050	G
10	13	2	5.0000	G
11	14	3	0.0050	G
12	14	2	5.0000	G
13	14	3	0.0050	G
14	14	2	5.0000	G
19	17	3	0.0250	G
20	17	2	25.0000	G
21	22	3	0.0050	G
22	22	2	5.0000	G
\.


--
-- TOC entry 5088 (class 0 OID 61096)
-- Dependencies: 233
-- Data for Name: order_items; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.order_items (order_item_id, order_id, batch_id, quantity, unit_price, units_per_batch, status) FROM stdin;
1	1	1	1	35.00	2	completed
2	1	2	1	110.50	2	completed
3	3	1	2	35.00	2	completed
10	7	25	1	55.50	5	canceled
6	5	4	4	108.80	4	canceled
7	5	1	2	35.00	2	canceled
12	9	25	1	55.50	5	completed
4	4	3	1	78.00	3	canceled
5	4	4	1	108.80	4	canceled
8	6	1	2	35.00	2	canceled
9	6	3	3	78.00	3	canceled
11	8	25	1	55.50	5	completed
13	10	25	1	55.50	5	canceled
14	11	25	1	55.50	5	completed
16	13	25	1	55.50	5	canceled
17	18	25	5	55.50	5	in_progress
15	12	1	1	28.00	2	canceled
18	19	27	1	28.00	4	created
19	20	27	2	28.00	4	created
20	21	2	2	143.00	2	created
21	22	25	1	55.50	5	created
23	24	25	5	55.50	5	created
24	25	4	2	140.80	4	created
25	26	26	1	130.00	5	created
26	26	2	1	143.00	2	created
27	27	4	2	140.80	4	created
28	27	1	1	28.00	2	created
29	28	4	2	140.80	4	created
30	28	26	1	130.00	5	created
22	23	25	1	55.50	5	in_progress
\.


--
-- TOC entry 5090 (class 0 OID 61101)
-- Dependencies: 235
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orders (order_id, start_date, end_date, created_at, company_id, canceled_at, cancel_reason, status) FROM stdin;
1	2026-04-03	2026-04-03	2026-04-01 10:30:00+05	1	\N	\N	completed
3	\N	2026-04-15	2026-04-14 08:47:08.878823+05	1	\N	\N	completed
7	2026-06-19	2026-05-21	2026-05-21 02:06:28.808671+05	1	2026-05-21 02:46:51.01601+05	\N	canceled
5	\N	2026-04-22	2026-04-14 11:52:27.35117+05	1	2026-05-21 02:52:57.905985+05	\N	canceled
9	2026-05-26	2026-05-26	2026-05-21 02:32:29.576362+05	1	\N	\N	completed
4	\N	2026-04-16	2026-04-14 08:48:22.926446+05	1	2026-05-21 03:09:52.056489+05	\N	canceled
6	\N	2026-04-28	2026-04-26 13:20:25.604024+05	1	2026-05-21 03:09:54.204127+05	\N	canceled
8	2026-06-19	2026-06-09	2026-05-21 02:24:48.551962+05	1	\N	\N	completed
10	2026-05-22	2026-05-24	2026-05-21 14:41:57.247502+05	1	2026-05-21 17:59:56.498132+05	\N	canceled
11	2026-05-22	2026-05-23	2026-05-21 14:42:15.111641+05	1	\N	\N	completed
13	2026-05-25	2026-05-25	2026-05-21 17:58:24.837688+05	1	2026-05-21 18:00:06.746747+05	\N	canceled
18	2026-06-18	2026-06-20	2026-05-21 20:32:52.791393+05	1	\N	\N	in_progress
15	\N	2026-06-20	2026-05-21 20:23:56.911837+05	1	2026-05-21 20:44:23.051743+05	\N	canceled
14	\N	2026-06-20	2026-05-21 20:22:49.535852+05	1	2026-05-21 20:45:20.566732+05	\N	canceled
12	\N	2026-05-25	2026-05-21 17:57:44.665666+05	1	2026-05-21 20:46:49.963742+05	\N	canceled
19	\N	2026-05-25	2026-05-22 00:19:28.905453+05	1	\N	\N	created
20	\N	2026-05-25	2026-05-22 00:20:39.057323+05	1	\N	\N	created
21	\N	2026-05-25	2026-05-22 00:21:36.625868+05	1	\N	\N	created
22	\N	2026-06-30	2026-05-22 00:30:38.502036+05	1	\N	\N	created
24	\N	2026-06-18	2026-05-22 00:32:39.690174+05	1	\N	\N	created
25	\N	2026-05-28	2026-05-22 00:33:22.765787+05	5	\N	\N	created
26	\N	2026-05-26	2026-05-22 00:34:55.158967+05	5	\N	\N	created
27	\N	2026-05-27	2026-05-22 00:36:37.718826+05	5	\N	\N	created
28	\N	2026-05-27	2026-05-22 00:38:54.106506+05	6	\N	\N	created
23	2026-06-30	2026-06-30	2026-05-22 00:31:48.071877+05	1	\N	\N	in_progress
\.


--
-- TOC entry 5092 (class 0 OID 61108)
-- Dependencies: 237
-- Data for Name: product_batches; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_batches (product_batch_id, product_id, quantity_units, markup_percent) FROM stdin;
2	2	2	30
3	3	3	20
4	2	4	28
8	7	10	80
9	7	4	100
22	12	2	120
23	12	5	100
24	12	10	70
25	13	5	11
26	14	5	30
28	15	3	4
29	16	6	4
1	1	2	50
27	1	4	25
\.


--
-- TOC entry 5094 (class 0 OID 61114)
-- Dependencies: 239
-- Data for Name: product_categories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_categories (product_category_id, name, color, deleted_at) FROM stdin;
1	Выпечка	FFD700	\N
2	Пирожные	FF69B4	\N
\.


--
-- TOC entry 5095 (class 0 OID 61117)
-- Dependencies: 240
-- Data for Name: product_images; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_images (product_image_id, product_id, image_path) FROM stdin;
1	1	sdobnaya_bulocha_s_koricei.jpg
2	2	ecler_s_zavarnym_kremom_prev.jpg
3	2	ecler_s_zavarnym_kremom_2.jpg
14	12	9221a88af9e44c1d860b0b31aca3d2fc.jpeg
16	12	98e1945640d24b43a323c53f0e8016c0.png
18	13	4302794d54e8418fa3d74188be0768c1.jpg
19	13	a767a3aff28742baa1d2ef841fa880be.jpg
20	13	807e751af28c4e1696ef3b4eb735c5a0.jpg
\.


--
-- TOC entry 5098 (class 0 OID 61124)
-- Dependencies: 243
-- Data for Name: products; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.products (product_id, category_id, name, description, cost_price, weight, production_time_minutes, deleted_at, storage_temp_min, storage_temp_max, shelf_life_days) FROM stdin;
1	1	Сдобная булочка с корицей	Сладкая булочка с корицей	28.00	0.090	30	\N	18.0	25.0	2
3	1	Круассан масляный с маком и яблоком	Слоеный круассан	65.00	0.070	45	\N	18.0	22.0	2
7	1	ваыв	ываыаы	40.00	50.000	20	2026-05-15 07:30:10.009056+05	2.0	-3.0	4
12	2	Яблочный пирожок	Очень вкусный нямнямням	40.00	50.000	20	2026-05-16 16:32:49.417552+05	2.0	-3.0	4
14	1	Яблочный пирог	Песочная нежная шарлотка. Приготовлена из свежих красных яблок 	100.00	50.000	30	\N	-5.0	5.0	11
2	1	Эклер 	Заварное пирожное	110.00	0.080	30	\N	2.0	6.0	3
13	2	Шоколадный торт	Вкусная ночинка с нежнейшим творогом и клубничной посыпкой	50.00	50.000	55	\N	-5.0	3.0	4
15	1	Эклер	фва	40.00	50.000	30	2026-05-22 02:09:01.794538+05	0.0	0.0	2
16	1	фыва	фыва	50.00	50.000	20	\N	0.0	0.0	4
\.


--
-- TOC entry 5100 (class 0 OID 61133)
-- Dependencies: 245
-- Data for Name: recipes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.recipes (recipe_id, product_id, ingredient_id, quantity) FROM stdin;
8	3	1	0.300
9	3	3	0.100
10	3	2	0.020
17	7	1	0.400
18	7	4	0.100
19	7	18	2.000
21	12	5	1.000
22	12	1	0.900
27	13	5	1.000
28	13	2	1.000
29	14	15	0.900
30	14	2	0.500
31	1	2	0.040
32	1	3	0.020
33	1	4	1.000
34	1	1	0.250
35	2	3	0.150
36	2	4	2.000
37	2	5	1.100
38	2	1	0.200
39	15	3	0.400
40	16	3	0.600
\.


--
-- TOC entry 5102 (class 0 OID 61137)
-- Dependencies: 247
-- Data for Name: supplier_ingredients; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.supplier_ingredients (supplier_ingredient_id, supplier_id, ingredient_id, name, price, weight, shelf_life_days, deleted_at, unit) FROM stdin;
1	3	1	Мука высший сорт	48.00	100.000000	12	\N	g
11	4	5	Молоко Простоквашено	50.00	30.000000	10	\N	ml
13	3	15	"Лукошкины Яблоки"	310.00	10.000000	30	\N	kg
3	3	5	Молоко Добрый Дом	300.00	1.000000	30	\N	l
2	3	2	Сахар	58.00	1.000000	24	\N	g
4	4	4	Яйца С1	75.00	6.000000	20	\N	kg
12	4	18	Яблоки "Добрый Фермер"	40.00	0.300000	40	\N	pcs
\.


--
-- TOC entry 5104 (class 0 OID 61141)
-- Dependencies: 249
-- Data for Name: suppliers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.suppliers (supplier_id, name, email, phone, address, deleted_at) FROM stdin;
2	Новый Поставщик	\N	88990800989		2026-05-03 21:09:38.124597+05
1	ООО "УралПродукт"	kasd@fdf.re	\N	г. Екатеринбург, ул. Складская, 12	2026-05-08 18:57:56.018267+05
4	ООО Россия	\N	\N	\N	\N
3	ТОКИО ХАБ	\N	89377880903	Октябрьский	\N
5	Фыва	\N	\N	\N	\N
\.


--
-- TOC entry 5106 (class 0 OID 61147)
-- Dependencies: 251
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (user_id, account_id, firstname, lastname, patronymic, birthday, role) FROM stdin;
1	2	Алексей	Воробьев	Иванович	1988-03-15	technologist
5	9	Касимов	Владлен	\N	2006-01-01	commercial_manager
6	10	Владимировна	Ольга	\N	1976-01-01	technologist
2	3	Марина	Штирлиц	Петровна	1993-07-22	commercial_manager
7	11	Примудрый	Владислав	\N	1966-01-01	commercial_manager
8	12	фывафваф	фывафыва	\N	1969-01-01	commercial_manager
9	13	фывафываыф	фывафыва	\N	1980-01-01	technologist
\.


--
-- TOC entry 5136 (class 0 OID 0)
-- Dependencies: 218
-- Name: accounts_account_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.accounts_account_id_seq', 15, true);


--
-- TOC entry 5137 (class 0 OID 0)
-- Dependencies: 220
-- Name: cart_items_new_cart_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.cart_items_new_cart_item_id_seq', 35, true);


--
-- TOC entry 5138 (class 0 OID 0)
-- Dependencies: 222
-- Name: companies_company_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.companies_company_id_seq', 6, true);


--
-- TOC entry 5139 (class 0 OID 0)
-- Dependencies: 224
-- Name: employee_tasks_new_employee_task_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employee_tasks_new_employee_task_id_seq', 38, true);


--
-- TOC entry 5140 (class 0 OID 0)
-- Dependencies: 226
-- Name: employees_employee_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employees_employee_id_seq', 11, true);


--
-- TOC entry 5141 (class 0 OID 0)
-- Dependencies: 228
-- Name: favourites_favourite_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.favourites_favourite_id_seq', 5, true);


--
-- TOC entry 5142 (class 0 OID 0)
-- Dependencies: 230
-- Name: ingredient_batches_ingredient_batch_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredient_batches_ingredient_batch_id_seq', 5, false);


--
-- TOC entry 5143 (class 0 OID 0)
-- Dependencies: 232
-- Name: ingredients_ingredient_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredients_ingredient_id_seq', 21, true);


--
-- TOC entry 5144 (class 0 OID 0)
-- Dependencies: 253
-- Name: order_item_ingredient_reserva_order_item_ingredient_reserva_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.order_item_ingredient_reserva_order_item_ingredient_reserva_seq', 22, true);


--
-- TOC entry 5145 (class 0 OID 0)
-- Dependencies: 234
-- Name: order_items_new_order_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.order_items_new_order_item_id_seq', 30, true);


--
-- TOC entry 5146 (class 0 OID 0)
-- Dependencies: 236
-- Name: orders_order_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orders_order_id_seq', 28, true);


--
-- TOC entry 5147 (class 0 OID 0)
-- Dependencies: 238
-- Name: product_batches_new_product_batch_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.product_batches_new_product_batch_id_seq', 29, true);


--
-- TOC entry 5148 (class 0 OID 0)
-- Dependencies: 241
-- Name: product_images_product_image_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.product_images_product_image_id_seq', 20, true);


--
-- TOC entry 5149 (class 0 OID 0)
-- Dependencies: 242
-- Name: production_categories_production_category_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.production_categories_production_category_id_seq', 3, false);


--
-- TOC entry 5150 (class 0 OID 0)
-- Dependencies: 244
-- Name: products_product_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.products_product_id_seq', 16, true);


--
-- TOC entry 5151 (class 0 OID 0)
-- Dependencies: 246
-- Name: recipes_recipe_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.recipes_recipe_id_seq', 40, true);


--
-- TOC entry 5152 (class 0 OID 0)
-- Dependencies: 248
-- Name: supplier_ingredient_supplier_ingredient_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.supplier_ingredient_supplier_ingredient_id_seq', 13, true);


--
-- TOC entry 5153 (class 0 OID 0)
-- Dependencies: 250
-- Name: suppliers_supplier_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.suppliers_supplier_id_seq', 5, true);


--
-- TOC entry 5154 (class 0 OID 0)
-- Dependencies: 252
-- Name: system_users_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.system_users_user_id_seq', 9, true);


--
-- TOC entry 4838 (class 2606 OID 61170)
-- Name: accounts accounts_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts
    ADD CONSTRAINT accounts_pkey PRIMARY KEY (account_id);


--
-- TOC entry 4840 (class 2606 OID 61172)
-- Name: cart_items cart_items_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT cart_items_new_pkey PRIMARY KEY (cart_item_id);


--
-- TOC entry 4844 (class 2606 OID 61174)
-- Name: companies companies_inn_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_inn_key UNIQUE (inn);


--
-- TOC entry 4846 (class 2606 OID 61176)
-- Name: companies companies_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_name_key UNIQUE (name);


--
-- TOC entry 4848 (class 2606 OID 61178)
-- Name: companies companies_ogrn_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_ogrn_key UNIQUE (ogrn);


--
-- TOC entry 4850 (class 2606 OID 61180)
-- Name: companies companies_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_pkey PRIMARY KEY (company_id);


--
-- TOC entry 4854 (class 2606 OID 61182)
-- Name: employee_tasks employee_tasks_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT employee_tasks_new_pkey PRIMARY KEY (employee_task_id);


--
-- TOC entry 4860 (class 2606 OID 61184)
-- Name: employees employees_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (employee_id);


--
-- TOC entry 4862 (class 2606 OID 61186)
-- Name: favorites favourites_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT favourites_pkey PRIMARY KEY (favorite_id);


--
-- TOC entry 4867 (class 2606 OID 61188)
-- Name: ingredient_batches ingredient_batches_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches
    ADD CONSTRAINT ingredient_batches_pkey PRIMARY KEY (ingredient_batch_id);


--
-- TOC entry 4869 (class 2606 OID 61190)
-- Name: ingredients ingredients_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients
    ADD CONSTRAINT ingredients_pkey PRIMARY KEY (ingredient_id);


--
-- TOC entry 4905 (class 2606 OID 61359)
-- Name: order_item_ingredient_reservations order_item_ingredient_reservations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_item_ingredient_reservations
    ADD CONSTRAINT order_item_ingredient_reservations_pkey PRIMARY KEY (order_item_ingredient_reservation_id);


--
-- TOC entry 4873 (class 2606 OID 61192)
-- Name: order_items order_items_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT order_items_new_pkey PRIMARY KEY (order_item_id);


--
-- TOC entry 4876 (class 2606 OID 61194)
-- Name: orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (order_id);


--
-- TOC entry 4879 (class 2606 OID 61196)
-- Name: product_batches product_batches_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches
    ADD CONSTRAINT product_batches_new_pkey PRIMARY KEY (product_batch_id);


--
-- TOC entry 4884 (class 2606 OID 61198)
-- Name: product_images product_images_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images
    ADD CONSTRAINT product_images_pkey PRIMARY KEY (product_image_id);


--
-- TOC entry 4881 (class 2606 OID 61200)
-- Name: product_categories production_categories_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_categories
    ADD CONSTRAINT production_categories_pkey PRIMARY KEY (product_category_id);


--
-- TOC entry 4887 (class 2606 OID 61202)
-- Name: products products_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT products_pkey PRIMARY KEY (product_id);


--
-- TOC entry 4891 (class 2606 OID 61204)
-- Name: recipes recipes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT recipes_pkey PRIMARY KEY (recipe_id);


--
-- TOC entry 4895 (class 2606 OID 61206)
-- Name: supplier_ingredients supplier_ingredient_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supplier_ingredients
    ADD CONSTRAINT supplier_ingredient_pkey PRIMARY KEY (supplier_ingredient_id);


--
-- TOC entry 4897 (class 2606 OID 61208)
-- Name: suppliers suppliers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suppliers
    ADD CONSTRAINT suppliers_pkey PRIMARY KEY (supplier_id);


--
-- TOC entry 4899 (class 2606 OID 61210)
-- Name: users system_users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT system_users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4852 (class 2606 OID 61212)
-- Name: companies unique_account_per_company; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT unique_account_per_company UNIQUE (account_id);


--
-- TOC entry 4901 (class 2606 OID 61214)
-- Name: users unique_account_per_user; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT unique_account_per_user UNIQUE (account_id);


--
-- TOC entry 4836 (class 1259 OID 61215)
-- Name: accounts_email_active_unique; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX accounts_email_active_unique ON public.accounts USING btree (((deleted_at IS NULL))) INCLUDE (deleted_at) WITH (deduplicate_items='true');


--
-- TOC entry 4841 (class 1259 OID 61216)
-- Name: fk_cart_items_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_cart_items_account_id_idx ON public.cart_items USING btree (company_id);


--
-- TOC entry 4842 (class 1259 OID 61217)
-- Name: fk_cart_items_product_batch_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_cart_items_product_batch_id_idx ON public.cart_items USING btree (batch_id);


--
-- TOC entry 4855 (class 1259 OID 61218)
-- Name: fk_employee_tasks_employee_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_employee_tasks_employee_id_idx ON public.employee_tasks USING btree (employee_id);


--
-- TOC entry 4856 (class 1259 OID 61219)
-- Name: fk_employee_tasks_order_item_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_employee_tasks_order_item_id_idx ON public.employee_tasks USING btree (order_item_id);


--
-- TOC entry 4863 (class 1259 OID 61220)
-- Name: fk_favorites_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_favorites_account_id_idx ON public.favorites USING btree (company_id);


--
-- TOC entry 4864 (class 1259 OID 61221)
-- Name: fk_favorites_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_favorites_product_id_idx ON public.favorites USING btree (product_id);


--
-- TOC entry 4870 (class 1259 OID 61222)
-- Name: fk_order_items_order_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_order_items_order_id_idx ON public.order_items USING btree (order_id);


--
-- TOC entry 4871 (class 1259 OID 61223)
-- Name: fk_order_items_product_batch_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_order_items_product_batch_id_idx ON public.order_items USING btree (batch_id);


--
-- TOC entry 4874 (class 1259 OID 61224)
-- Name: fk_orders_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_orders_account_id_idx ON public.orders USING btree (company_id);


--
-- TOC entry 4877 (class 1259 OID 61225)
-- Name: fk_product_batches_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_product_batches_product_id_idx ON public.product_batches USING btree (product_id);


--
-- TOC entry 4882 (class 1259 OID 61226)
-- Name: fk_product_images_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_product_images_product_id_idx ON public.product_images USING btree (product_id);


--
-- TOC entry 4885 (class 1259 OID 61227)
-- Name: fk_products_category_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_products_category_id_idx ON public.products USING btree (category_id);


--
-- TOC entry 4888 (class 1259 OID 61228)
-- Name: fk_recipe_ingredient_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_recipe_ingredient_id_idx ON public.recipes USING btree (ingredient_id);


--
-- TOC entry 4889 (class 1259 OID 61229)
-- Name: fk_recipe_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_recipe_product_id_idx ON public.recipes USING btree (product_id);


--
-- TOC entry 4857 (class 1259 OID 61230)
-- Name: idx_employee_tasks_employee_end; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_employee_tasks_employee_end ON public.employee_tasks USING btree (employee_id, end_time);


--
-- TOC entry 4858 (class 1259 OID 61231)
-- Name: idx_employee_tasks_employee_start; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_employee_tasks_employee_start ON public.employee_tasks USING btree (employee_id, start_time);


--
-- TOC entry 4865 (class 1259 OID 61232)
-- Name: idx_ingredient_batches_supplier_ingredient; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_ingredient_batches_supplier_ingredient ON public.ingredient_batches USING btree (supplier_ingredient_id);


--
-- TOC entry 4902 (class 1259 OID 61371)
-- Name: idx_reservations_ingredient_batch; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_reservations_ingredient_batch ON public.order_item_ingredient_reservations USING btree (ingredient_batch_id);


--
-- TOC entry 4903 (class 1259 OID 61370)
-- Name: idx_reservations_order_item; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_reservations_order_item ON public.order_item_ingredient_reservations USING btree (order_item_id);


--
-- TOC entry 4892 (class 1259 OID 61233)
-- Name: idx_supplier_ingredient_ingredient; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_supplier_ingredient_ingredient ON public.supplier_ingredients USING btree (ingredient_id);


--
-- TOC entry 4893 (class 1259 OID 61234)
-- Name: idx_supplier_ingredient_supplier; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_supplier_ingredient_supplier ON public.supplier_ingredients USING btree (supplier_id);


--
-- TOC entry 4908 (class 2606 OID 61235)
-- Name: companies companies_account_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_account_id_fkey FOREIGN KEY (account_id) REFERENCES public.accounts(account_id) ON DELETE CASCADE;


--
-- TOC entry 4906 (class 2606 OID 61240)
-- Name: cart_items fk_cart_items_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT fk_cart_items_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4907 (class 2606 OID 61245)
-- Name: cart_items fk_cart_items_product_batch_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT fk_cart_items_product_batch_id FOREIGN KEY (batch_id) REFERENCES public.product_batches(product_batch_id);


--
-- TOC entry 4909 (class 2606 OID 61250)
-- Name: employee_tasks fk_employee_tasks_employee_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT fk_employee_tasks_employee_id FOREIGN KEY (employee_id) REFERENCES public.employees(employee_id);


--
-- TOC entry 4910 (class 2606 OID 61255)
-- Name: employee_tasks fk_employee_tasks_order_item_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT fk_employee_tasks_order_item_id FOREIGN KEY (order_item_id) REFERENCES public.order_items(order_item_id);


--
-- TOC entry 4911 (class 2606 OID 61260)
-- Name: favorites fk_favourites_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT fk_favourites_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4912 (class 2606 OID 61265)
-- Name: favorites fk_favourites_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT fk_favourites_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4913 (class 2606 OID 61270)
-- Name: ingredient_batches fk_ingredient_batches_supplier_ingredient; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches
    ADD CONSTRAINT fk_ingredient_batches_supplier_ingredient FOREIGN KEY (supplier_ingredient_id) REFERENCES public.supplier_ingredients(supplier_ingredient_id);


--
-- TOC entry 4914 (class 2606 OID 61275)
-- Name: order_items fk_order_items_order_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT fk_order_items_order_id FOREIGN KEY (order_id) REFERENCES public.orders(order_id);


--
-- TOC entry 4915 (class 2606 OID 61280)
-- Name: order_items fk_order_items_product_batch_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT fk_order_items_product_batch_id FOREIGN KEY (batch_id) REFERENCES public.product_batches(product_batch_id);


--
-- TOC entry 4916 (class 2606 OID 61285)
-- Name: orders fk_orders_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT fk_orders_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4917 (class 2606 OID 61290)
-- Name: product_batches fk_product_batches_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches
    ADD CONSTRAINT fk_product_batches_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4918 (class 2606 OID 61295)
-- Name: product_images fk_product_images_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images
    ADD CONSTRAINT fk_product_images_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4919 (class 2606 OID 61300)
-- Name: products fk_products_category_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT fk_products_category_id FOREIGN KEY (category_id) REFERENCES public.product_categories(product_category_id);


--
-- TOC entry 4920 (class 2606 OID 61305)
-- Name: recipes fk_recipe_ingredient_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_recipe_ingredient_id FOREIGN KEY (ingredient_id) REFERENCES public.ingredients(ingredient_id);


--
-- TOC entry 4921 (class 2606 OID 61310)
-- Name: recipes fk_recipe_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_recipe_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4925 (class 2606 OID 61365)
-- Name: order_item_ingredient_reservations fk_reservation_ingredient_batch; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_item_ingredient_reservations
    ADD CONSTRAINT fk_reservation_ingredient_batch FOREIGN KEY (ingredient_batch_id) REFERENCES public.ingredient_batches(ingredient_batch_id) ON DELETE RESTRICT;


--
-- TOC entry 4926 (class 2606 OID 61360)
-- Name: order_item_ingredient_reservations fk_reservation_order_item; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_item_ingredient_reservations
    ADD CONSTRAINT fk_reservation_order_item FOREIGN KEY (order_item_id) REFERENCES public.order_items(order_item_id) ON DELETE CASCADE;


--
-- TOC entry 4922 (class 2606 OID 61315)
-- Name: supplier_ingredients fk_supplier_ingredient_ingredient; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supplier_ingredients
    ADD CONSTRAINT fk_supplier_ingredient_ingredient FOREIGN KEY (ingredient_id) REFERENCES public.ingredients(ingredient_id);


--
-- TOC entry 4923 (class 2606 OID 61320)
-- Name: supplier_ingredients fk_supplier_ingredient_supplier; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supplier_ingredients
    ADD CONSTRAINT fk_supplier_ingredient_supplier FOREIGN KEY (supplier_id) REFERENCES public.suppliers(supplier_id);


--
-- TOC entry 4924 (class 2606 OID 61325)
-- Name: users system_users_account_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT system_users_account_id_fkey FOREIGN KEY (account_id) REFERENCES public.accounts(account_id) ON DELETE CASCADE;


--
-- TOC entry 5116 (class 0 OID 0)
-- Dependencies: 5
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;


-- Completed on 2026-05-22 04:35:49

--
-- PostgreSQL database dump complete
--

\unrestrict 5JjS6p4540QkWPz4f3cYUzNGDkZKnnbMdN1CRtkMe4DVVXLggQwoCRgTuJAitQx

