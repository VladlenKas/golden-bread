--
-- PostgreSQL database dump
--

\restrict cU1fjRbGQ7MSh40SPT1dmoNGrSwo5jZCD1lwvSZtNuPlb3tqMnfYEamJPXAmvrr

-- Dumped from database version 17.6
-- Dumped by pg_dump version 17.6

-- Started on 2026-05-22 05:37:37

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
-- TOC entry 883 (class 1247 OID 61688)
-- Name: account_type; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.account_type AS ENUM (
    'user',
    'company'
);


ALTER TYPE public.account_type OWNER TO postgres;

--
-- TOC entry 886 (class 1247 OID 61694)
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
-- TOC entry 889 (class 1247 OID 61702)
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
-- TOC entry 892 (class 1247 OID 61714)
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
-- TOC entry 895 (class 1247 OID 61724)
-- Name: user_role; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.user_role AS ENUM (
    'commercial_manager',
    'technologist'
);


ALTER TYPE public.user_role OWNER TO postgres;

--
-- TOC entry 898 (class 1247 OID 61730)
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
-- TOC entry 217 (class 1259 OID 61739)
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
-- TOC entry 218 (class 1259 OID 61746)
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
-- TOC entry 5115 (class 0 OID 0)
-- Dependencies: 218
-- Name: accounts_account_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.accounts_account_id_seq OWNED BY public.accounts.account_id;


--
-- TOC entry 219 (class 1259 OID 61747)
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
-- TOC entry 220 (class 1259 OID 61750)
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
-- TOC entry 5116 (class 0 OID 0)
-- Dependencies: 220
-- Name: cart_items_new_cart_item_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cart_items_new_cart_item_id_seq OWNED BY public.cart_items.cart_item_id;


--
-- TOC entry 221 (class 1259 OID 61751)
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
-- TOC entry 222 (class 1259 OID 61756)
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
-- TOC entry 5117 (class 0 OID 0)
-- Dependencies: 222
-- Name: companies_company_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.companies_company_id_seq OWNED BY public.companies.company_id;


--
-- TOC entry 223 (class 1259 OID 61757)
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
-- TOC entry 224 (class 1259 OID 61763)
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
-- TOC entry 5118 (class 0 OID 0)
-- Dependencies: 224
-- Name: employee_tasks_new_employee_task_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employee_tasks_new_employee_task_id_seq OWNED BY public.employee_tasks.employee_task_id;


--
-- TOC entry 225 (class 1259 OID 61764)
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
-- TOC entry 226 (class 1259 OID 61767)
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
-- TOC entry 5119 (class 0 OID 0)
-- Dependencies: 226
-- Name: employees_employee_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employees_employee_id_seq OWNED BY public.employees.employee_id;


--
-- TOC entry 227 (class 1259 OID 61768)
-- Name: favorites; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.favorites (
    favorite_id integer NOT NULL,
    product_id integer NOT NULL,
    company_id integer
);


ALTER TABLE public.favorites OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 61771)
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
-- TOC entry 5120 (class 0 OID 0)
-- Dependencies: 228
-- Name: favourites_favourite_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.favourites_favourite_id_seq OWNED BY public.favorites.favorite_id;


--
-- TOC entry 229 (class 1259 OID 61772)
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
-- TOC entry 230 (class 1259 OID 61775)
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
-- TOC entry 5121 (class 0 OID 0)
-- Dependencies: 230
-- Name: ingredient_batches_ingredient_batch_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredient_batches_ingredient_batch_id_seq OWNED BY public.ingredient_batches.ingredient_batch_id;


--
-- TOC entry 231 (class 1259 OID 61776)
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
-- TOC entry 232 (class 1259 OID 61779)
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
-- TOC entry 5122 (class 0 OID 0)
-- Dependencies: 232
-- Name: ingredients_ingredient_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredients_ingredient_id_seq OWNED BY public.ingredients.ingredient_id;


--
-- TOC entry 254 (class 1259 OID 62024)
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
-- TOC entry 253 (class 1259 OID 62023)
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
-- TOC entry 5123 (class 0 OID 0)
-- Dependencies: 253
-- Name: order_item_ingredient_reserva_order_item_ingredient_reserva_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.order_item_ingredient_reserva_order_item_ingredient_reserva_seq OWNED BY public.order_item_ingredient_reservations.order_item_ingredient_reservation_id;


--
-- TOC entry 233 (class 1259 OID 61780)
-- Name: order_items; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.order_items (
    order_item_id integer NOT NULL,
    order_id integer NOT NULL,
    batch_id integer NOT NULL,
    status public.order_status NOT NULL,
    quantity integer NOT NULL,
    unit_price numeric(10,2) DEFAULT 0 NOT NULL,
    units_per_batch integer NOT NULL
);


ALTER TABLE public.order_items OWNER TO postgres;

--
-- TOC entry 234 (class 1259 OID 61784)
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
-- TOC entry 5124 (class 0 OID 0)
-- Dependencies: 234
-- Name: order_items_new_order_item_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.order_items_new_order_item_id_seq OWNED BY public.order_items.order_item_id;


--
-- TOC entry 235 (class 1259 OID 61785)
-- Name: orders; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.orders (
    order_id integer NOT NULL,
    status public.order_status NOT NULL,
    start_date date,
    end_date date NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    company_id integer NOT NULL,
    canceled_at timestamp with time zone,
    cancel_reason text
);


ALTER TABLE public.orders OWNER TO postgres;

--
-- TOC entry 236 (class 1259 OID 61791)
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
-- TOC entry 5125 (class 0 OID 0)
-- Dependencies: 236
-- Name: orders_order_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.orders_order_id_seq OWNED BY public.orders.order_id;


--
-- TOC entry 237 (class 1259 OID 61792)
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
-- TOC entry 238 (class 1259 OID 61797)
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
-- TOC entry 5126 (class 0 OID 0)
-- Dependencies: 238
-- Name: product_batches_new_product_batch_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.product_batches_new_product_batch_id_seq OWNED BY public.product_batches.product_batch_id;


--
-- TOC entry 239 (class 1259 OID 61798)
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
-- TOC entry 240 (class 1259 OID 61801)
-- Name: product_images; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product_images (
    product_image_id integer NOT NULL,
    product_id integer NOT NULL,
    image_path character varying
);


ALTER TABLE public.product_images OWNER TO postgres;

--
-- TOC entry 241 (class 1259 OID 61806)
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
-- TOC entry 5127 (class 0 OID 0)
-- Dependencies: 241
-- Name: product_images_product_image_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.product_images_product_image_id_seq OWNED BY public.product_images.product_image_id;


--
-- TOC entry 242 (class 1259 OID 61807)
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
-- TOC entry 5128 (class 0 OID 0)
-- Dependencies: 242
-- Name: production_categories_production_category_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.production_categories_production_category_id_seq OWNED BY public.product_categories.product_category_id;


--
-- TOC entry 243 (class 1259 OID 61808)
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
-- TOC entry 244 (class 1259 OID 61816)
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
-- TOC entry 5129 (class 0 OID 0)
-- Dependencies: 244
-- Name: products_product_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.products_product_id_seq OWNED BY public.products.product_id;


--
-- TOC entry 245 (class 1259 OID 61817)
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
-- TOC entry 246 (class 1259 OID 61820)
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
-- TOC entry 5130 (class 0 OID 0)
-- Dependencies: 246
-- Name: recipes_recipe_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.recipes_recipe_id_seq OWNED BY public.recipes.recipe_id;


--
-- TOC entry 247 (class 1259 OID 61821)
-- Name: supplier_ingredients; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.supplier_ingredients (
    supplier_ingredient_id integer NOT NULL,
    supplier_id integer NOT NULL,
    ingredient_id integer NOT NULL,
    name character varying(100) NOT NULL,
    price numeric(10,2) NOT NULL,
    unit public.ingredient_unit NOT NULL,
    weight numeric(10,6) NOT NULL,
    shelf_life_days integer NOT NULL,
    deleted_at timestamp with time zone
);


ALTER TABLE public.supplier_ingredients OWNER TO postgres;

--
-- TOC entry 248 (class 1259 OID 61824)
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
-- TOC entry 5131 (class 0 OID 0)
-- Dependencies: 248
-- Name: supplier_ingredient_supplier_ingredient_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.supplier_ingredient_supplier_ingredient_id_seq OWNED BY public.supplier_ingredients.supplier_ingredient_id;


--
-- TOC entry 249 (class 1259 OID 61825)
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
-- TOC entry 250 (class 1259 OID 61830)
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
-- TOC entry 5132 (class 0 OID 0)
-- Dependencies: 250
-- Name: suppliers_supplier_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.suppliers_supplier_id_seq OWNED BY public.suppliers.supplier_id;


--
-- TOC entry 251 (class 1259 OID 61831)
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
-- TOC entry 252 (class 1259 OID 61834)
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
-- TOC entry 5133 (class 0 OID 0)
-- Dependencies: 252
-- Name: system_users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.system_users_user_id_seq OWNED BY public.users.user_id;


--
-- TOC entry 4803 (class 2604 OID 61835)
-- Name: accounts account_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts ALTER COLUMN account_id SET DEFAULT nextval('public.accounts_account_id_seq'::regclass);


--
-- TOC entry 4806 (class 2604 OID 61836)
-- Name: cart_items cart_item_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items ALTER COLUMN cart_item_id SET DEFAULT nextval('public.cart_items_new_cart_item_id_seq'::regclass);


--
-- TOC entry 4807 (class 2604 OID 61837)
-- Name: companies company_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies ALTER COLUMN company_id SET DEFAULT nextval('public.companies_company_id_seq'::regclass);


--
-- TOC entry 4808 (class 2604 OID 61838)
-- Name: employee_tasks employee_task_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks ALTER COLUMN employee_task_id SET DEFAULT nextval('public.employee_tasks_new_employee_task_id_seq'::regclass);


--
-- TOC entry 4810 (class 2604 OID 61839)
-- Name: employees employee_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees ALTER COLUMN employee_id SET DEFAULT nextval('public.employees_employee_id_seq'::regclass);


--
-- TOC entry 4811 (class 2604 OID 61840)
-- Name: favorites favorite_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites ALTER COLUMN favorite_id SET DEFAULT nextval('public.favourites_favourite_id_seq'::regclass);


--
-- TOC entry 4812 (class 2604 OID 61841)
-- Name: ingredient_batches ingredient_batch_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches ALTER COLUMN ingredient_batch_id SET DEFAULT nextval('public.ingredient_batches_ingredient_batch_id_seq'::regclass);


--
-- TOC entry 4813 (class 2604 OID 61842)
-- Name: ingredients ingredient_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients ALTER COLUMN ingredient_id SET DEFAULT nextval('public.ingredients_ingredient_id_seq'::regclass);


--
-- TOC entry 4830 (class 2604 OID 62027)
-- Name: order_item_ingredient_reservations order_item_ingredient_reservation_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_item_ingredient_reservations ALTER COLUMN order_item_ingredient_reservation_id SET DEFAULT nextval('public.order_item_ingredient_reserva_order_item_ingredient_reserva_seq'::regclass);


--
-- TOC entry 4814 (class 2604 OID 61843)
-- Name: order_items order_item_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items ALTER COLUMN order_item_id SET DEFAULT nextval('public.order_items_new_order_item_id_seq'::regclass);


--
-- TOC entry 4816 (class 2604 OID 61844)
-- Name: orders order_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders ALTER COLUMN order_id SET DEFAULT nextval('public.orders_order_id_seq'::regclass);


--
-- TOC entry 4818 (class 2604 OID 61845)
-- Name: product_batches product_batch_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches ALTER COLUMN product_batch_id SET DEFAULT nextval('public.product_batches_new_product_batch_id_seq'::regclass);


--
-- TOC entry 4820 (class 2604 OID 61846)
-- Name: product_categories product_category_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_categories ALTER COLUMN product_category_id SET DEFAULT nextval('public.production_categories_production_category_id_seq'::regclass);


--
-- TOC entry 4821 (class 2604 OID 61847)
-- Name: product_images product_image_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images ALTER COLUMN product_image_id SET DEFAULT nextval('public.product_images_product_image_id_seq'::regclass);


--
-- TOC entry 4822 (class 2604 OID 61848)
-- Name: products product_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products ALTER COLUMN product_id SET DEFAULT nextval('public.products_product_id_seq'::regclass);


--
-- TOC entry 4826 (class 2604 OID 61849)
-- Name: recipes recipe_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes ALTER COLUMN recipe_id SET DEFAULT nextval('public.recipes_recipe_id_seq'::regclass);


--
-- TOC entry 4827 (class 2604 OID 61850)
-- Name: supplier_ingredients supplier_ingredient_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supplier_ingredients ALTER COLUMN supplier_ingredient_id SET DEFAULT nextval('public.supplier_ingredient_supplier_ingredient_id_seq'::regclass);


--
-- TOC entry 4828 (class 2604 OID 61851)
-- Name: suppliers supplier_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suppliers ALTER COLUMN supplier_id SET DEFAULT nextval('public.suppliers_supplier_id_seq'::regclass);


--
-- TOC entry 4829 (class 2604 OID 61852)
-- Name: users user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.system_users_user_id_seq'::regclass);


--
-- TOC entry 5072 (class 0 OID 61739)
-- Dependencies: 217
-- Data for Name: accounts; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.accounts (account_id, email, password_hash, account_type, verification_status, session, session_expires_at, deleted_at, created_at) FROM stdin;
1	company@bakery.ru	$2a$11$mBkW8NaQM.Kc0/lvS1ik/.U3fgSBa98OmeIozzq9a.8AHpPvE4F4W	company	approved	f8094ad765344ddeb787dd40a3625bbf	2026-05-23 05:00:22.842571+05	\N	2026-04-01 00:00:00+05
2	tech@bakery.ru	$2a$11$qWbk77OYChBocx14lkAMBe0xe3hDL09aA8og/el9RCugaSYA8EUY.	user	approved	\N	\N	\N	2026-04-01 00:00:00+05
3	comm@bakery.ru	$2a$11$29Kij2EN7VlJm8gbOPR5TudpWZHKqBeoNc8cWVLVBRiA7wHVQSDvK	user	approved	dc069758313c4d50b4c45a0790091ee7	2026-05-23 05:18:47.60036+05	\N	2026-04-01 00:00:00+05
\.


--
-- TOC entry 5074 (class 0 OID 61747)
-- Dependencies: 219
-- Data for Name: cart_items; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.cart_items (cart_item_id, batch_id, quantity, company_id) FROM stdin;
1	3	1	1
2	4	1	1
\.


--
-- TOC entry 5076 (class 0 OID 61751)
-- Dependencies: 221
-- Data for Name: companies; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.companies (company_id, account_id, name, inn, ogrn, phone, address) FROM stdin;
1	1	ООО "Хлебное Место"	5793851133	5264386528962	83431234567	г. Екатеринбург, ул. Хлебозаводская, 5
\.


--
-- TOC entry 5078 (class 0 OID 61757)
-- Dependencies: 223
-- Data for Name: employee_tasks; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employee_tasks (employee_task_id, employee_id, order_item_id, status, assigned_quantity, completed_quantity, start_time, end_time) FROM stdin;
3	1	1	in_progress	1	0	\N	\N
4	2	1	in_progress	1	0	\N	\N
1	1	2	in_progress	1	0	\N	\N
2	2	2	in_progress	1	0	\N	\N
\.


--
-- TOC entry 5080 (class 0 OID 61764)
-- Dependencies: 225
-- Data for Name: employees; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employees (employee_id, firstname, lastname, patronymic, birthday, deleted_at) FROM stdin;
1	Иван	Петров	Сергеевич	1985-01-10	\N
2	Петр	Васильев	Алексеевич	1987-05-20	\N
\.


--
-- TOC entry 5082 (class 0 OID 61768)
-- Dependencies: 227
-- Data for Name: favorites; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.favorites (favorite_id, product_id, company_id) FROM stdin;
1	3	1
\.


--
-- TOC entry 5084 (class 0 OID 61772)
-- Dependencies: 229
-- Data for Name: ingredient_batches; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredient_batches (ingredient_batch_id, status, purchased_quantity, remaining_quantity, delivery_date, expiry_date, supplier_ingredient_id) FROM stdin;
1	available	100	100.000	2026-05-22	2026-11-22	1
2	available	50	50.000	2026-05-22	2026-11-22	2
3	available	30	30.000	2026-05-22	2026-11-22	3
4	available	200	200.000	2026-05-22	2026-11-22	4
\.


--
-- TOC entry 5086 (class 0 OID 61776)
-- Dependencies: 231
-- Data for Name: ingredients; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredients (ingredient_id, name, deleted_at, base_unit) FROM stdin;
1	Мука пшеничная	\N	kg
2	Сахар-песок	\N	kg
3	Масло сливочное	\N	kg
4	Яйца куриные	\N	kg
\.


--
-- TOC entry 5109 (class 0 OID 62024)
-- Dependencies: 254
-- Data for Name: order_item_ingredient_reservations; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.order_item_ingredient_reservations (order_item_ingredient_reservation_id, order_item_id, ingredient_batch_id, reserved_quantity, reserved_unit) FROM stdin;
\.


--
-- TOC entry 5088 (class 0 OID 61780)
-- Dependencies: 233
-- Data for Name: order_items; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.order_items (order_item_id, order_id, batch_id, status, quantity, unit_price, units_per_batch) FROM stdin;
1	1	1	in_progress	1	35.00	2
2	1	2	in_progress	1	110.50	2
\.


--
-- TOC entry 5090 (class 0 OID 61785)
-- Dependencies: 235
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orders (order_id, status, start_date, end_date, created_at, company_id, canceled_at, cancel_reason) FROM stdin;
1	in_progress	2026-04-03	2026-04-03	2026-04-01 10:30:00+05	1	\N	\N
\.


--
-- TOC entry 5092 (class 0 OID 61792)
-- Dependencies: 237
-- Data for Name: product_batches; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_batches (product_batch_id, product_id, quantity_units, markup_percent) FROM stdin;
1	1	2	25
2	2	2	30
3	3	3	20
4	2	4	28
\.


--
-- TOC entry 5094 (class 0 OID 61798)
-- Dependencies: 239
-- Data for Name: product_categories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_categories (product_category_id, name, color, deleted_at) FROM stdin;
1	Выпечка	FFD700	\N
2	Пирожные	FF69B4	\N
\.


--
-- TOC entry 5095 (class 0 OID 61801)
-- Dependencies: 240
-- Data for Name: product_images; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_images (product_image_id, product_id, image_path) FROM stdin;
1	1	sdobnaya_bulocha_s_koricei.jpg
2	2	ecler_s_zavarnym_kremom_prev.jpg
3	2	ecler_s_zavarnym_kremom_2.jpg
\.


--
-- TOC entry 5098 (class 0 OID 61808)
-- Dependencies: 243
-- Data for Name: products; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.products (product_id, category_id, name, description, cost_price, weight, production_time_minutes, deleted_at, storage_temp_min, storage_temp_max, shelf_life_days) FROM stdin;
1	1	Сдобная булочка с корицей	Сладкая булочка с корицей	28.00	0.090	30	\N	18.0	25.0	2
2	2	Эклер с заварным кремом	Заварное пирожное	85.00	0.080	30	\N	2.0	6.0	3
3	1	Круассан масляный	Слоеный круассан	65.00	0.070	45	\N	18.0	22.0	2
\.


--
-- TOC entry 5100 (class 0 OID 61817)
-- Dependencies: 245
-- Data for Name: recipes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.recipes (recipe_id, product_id, ingredient_id, quantity) FROM stdin;
1	1	1	0.250
2	1	2	0.040
3	1	3	0.020
4	1	4	1.000
5	2	1	0.200
6	2	3	0.050
7	2	4	2.000
8	3	1	0.300
9	3	3	0.100
10	3	2	0.020
\.


--
-- TOC entry 5102 (class 0 OID 61821)
-- Dependencies: 247
-- Data for Name: supplier_ingredients; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.supplier_ingredients (supplier_ingredient_id, supplier_id, ingredient_id, name, price, unit, weight, shelf_life_days, deleted_at) FROM stdin;
4	1	4	Яйца С1	75.00	pcs	0.060000	2	\N
1	1	1	Мука высший сорт	48.00	kg	1.000000	12	\N
2	1	2	Сахар белый	58.00	kg	1.000000	24	\N
3	1	3	Масло 82%	420.00	kg	1.000000	6	\N
\.


--
-- TOC entry 5104 (class 0 OID 61825)
-- Dependencies: 249
-- Data for Name: suppliers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.suppliers (supplier_id, name, email, phone, address, deleted_at) FROM stdin;
1	ООО "УралПродукт"	sales@uralprod.ru	83432165487	г. Екатеринбург, ул. Складская, 12	\N
\.


--
-- TOC entry 5106 (class 0 OID 61831)
-- Dependencies: 251
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (user_id, account_id, firstname, lastname, patronymic, birthday, role) FROM stdin;
1	2	Алексей	Воробьев	Иванович	1988-03-15	technologist
2	3	Марина	Соколова	Петровна	1992-07-22	commercial_manager
\.


--
-- TOC entry 5134 (class 0 OID 0)
-- Dependencies: 218
-- Name: accounts_account_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.accounts_account_id_seq', 3, true);


--
-- TOC entry 5135 (class 0 OID 0)
-- Dependencies: 220
-- Name: cart_items_new_cart_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.cart_items_new_cart_item_id_seq', 2, true);


--
-- TOC entry 5136 (class 0 OID 0)
-- Dependencies: 222
-- Name: companies_company_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.companies_company_id_seq', 1, true);


--
-- TOC entry 5137 (class 0 OID 0)
-- Dependencies: 224
-- Name: employee_tasks_new_employee_task_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employee_tasks_new_employee_task_id_seq', 4, true);


--
-- TOC entry 5138 (class 0 OID 0)
-- Dependencies: 226
-- Name: employees_employee_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employees_employee_id_seq', 3, true);


--
-- TOC entry 5139 (class 0 OID 0)
-- Dependencies: 228
-- Name: favourites_favourite_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.favourites_favourite_id_seq', 1, true);


--
-- TOC entry 5140 (class 0 OID 0)
-- Dependencies: 230
-- Name: ingredient_batches_ingredient_batch_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredient_batches_ingredient_batch_id_seq', 4, true);


--
-- TOC entry 5141 (class 0 OID 0)
-- Dependencies: 232
-- Name: ingredients_ingredient_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredients_ingredient_id_seq', 4, true);


--
-- TOC entry 5142 (class 0 OID 0)
-- Dependencies: 253
-- Name: order_item_ingredient_reserva_order_item_ingredient_reserva_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.order_item_ingredient_reserva_order_item_ingredient_reserva_seq', 1, false);


--
-- TOC entry 5143 (class 0 OID 0)
-- Dependencies: 234
-- Name: order_items_new_order_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.order_items_new_order_item_id_seq', 3, true);


--
-- TOC entry 5144 (class 0 OID 0)
-- Dependencies: 236
-- Name: orders_order_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orders_order_id_seq', 3, true);


--
-- TOC entry 5145 (class 0 OID 0)
-- Dependencies: 238
-- Name: product_batches_new_product_batch_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.product_batches_new_product_batch_id_seq', 4, true);


--
-- TOC entry 5146 (class 0 OID 0)
-- Dependencies: 241
-- Name: product_images_product_image_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.product_images_product_image_id_seq', 3, true);


--
-- TOC entry 5147 (class 0 OID 0)
-- Dependencies: 242
-- Name: production_categories_production_category_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.production_categories_production_category_id_seq', 2, true);


--
-- TOC entry 5148 (class 0 OID 0)
-- Dependencies: 244
-- Name: products_product_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.products_product_id_seq', 3, true);


--
-- TOC entry 5149 (class 0 OID 0)
-- Dependencies: 246
-- Name: recipes_recipe_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.recipes_recipe_id_seq', 10, true);


--
-- TOC entry 5150 (class 0 OID 0)
-- Dependencies: 248
-- Name: supplier_ingredient_supplier_ingredient_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.supplier_ingredient_supplier_ingredient_id_seq', 4, true);


--
-- TOC entry 5151 (class 0 OID 0)
-- Dependencies: 250
-- Name: suppliers_supplier_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.suppliers_supplier_id_seq', 1, true);


--
-- TOC entry 5152 (class 0 OID 0)
-- Dependencies: 252
-- Name: system_users_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.system_users_user_id_seq', 2, true);


--
-- TOC entry 4838 (class 2606 OID 61854)
-- Name: accounts accounts_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts
    ADD CONSTRAINT accounts_pkey PRIMARY KEY (account_id);


--
-- TOC entry 4840 (class 2606 OID 61856)
-- Name: cart_items cart_items_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT cart_items_new_pkey PRIMARY KEY (cart_item_id);


--
-- TOC entry 4844 (class 2606 OID 61858)
-- Name: companies companies_inn_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_inn_key UNIQUE (inn);


--
-- TOC entry 4846 (class 2606 OID 61860)
-- Name: companies companies_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_name_key UNIQUE (name);


--
-- TOC entry 4848 (class 2606 OID 61862)
-- Name: companies companies_ogrn_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_ogrn_key UNIQUE (ogrn);


--
-- TOC entry 4850 (class 2606 OID 61864)
-- Name: companies companies_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_pkey PRIMARY KEY (company_id);


--
-- TOC entry 4854 (class 2606 OID 61866)
-- Name: employee_tasks employee_tasks_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT employee_tasks_new_pkey PRIMARY KEY (employee_task_id);


--
-- TOC entry 4860 (class 2606 OID 61868)
-- Name: employees employees_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (employee_id);


--
-- TOC entry 4862 (class 2606 OID 61870)
-- Name: favorites favourites_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT favourites_pkey PRIMARY KEY (favorite_id);


--
-- TOC entry 4867 (class 2606 OID 61872)
-- Name: ingredient_batches ingredient_batches_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches
    ADD CONSTRAINT ingredient_batches_pkey PRIMARY KEY (ingredient_batch_id);


--
-- TOC entry 4869 (class 2606 OID 61874)
-- Name: ingredients ingredients_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients
    ADD CONSTRAINT ingredients_pkey PRIMARY KEY (ingredient_id);


--
-- TOC entry 4905 (class 2606 OID 62031)
-- Name: order_item_ingredient_reservations order_item_ingredient_reservations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_item_ingredient_reservations
    ADD CONSTRAINT order_item_ingredient_reservations_pkey PRIMARY KEY (order_item_ingredient_reservation_id);


--
-- TOC entry 4873 (class 2606 OID 61876)
-- Name: order_items order_items_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT order_items_new_pkey PRIMARY KEY (order_item_id);


--
-- TOC entry 4876 (class 2606 OID 61878)
-- Name: orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (order_id);


--
-- TOC entry 4879 (class 2606 OID 61880)
-- Name: product_batches product_batches_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches
    ADD CONSTRAINT product_batches_new_pkey PRIMARY KEY (product_batch_id);


--
-- TOC entry 4884 (class 2606 OID 61882)
-- Name: product_images product_images_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images
    ADD CONSTRAINT product_images_pkey PRIMARY KEY (product_image_id);


--
-- TOC entry 4881 (class 2606 OID 61884)
-- Name: product_categories production_categories_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_categories
    ADD CONSTRAINT production_categories_pkey PRIMARY KEY (product_category_id);


--
-- TOC entry 4887 (class 2606 OID 61886)
-- Name: products products_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT products_pkey PRIMARY KEY (product_id);


--
-- TOC entry 4891 (class 2606 OID 61888)
-- Name: recipes recipes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT recipes_pkey PRIMARY KEY (recipe_id);


--
-- TOC entry 4895 (class 2606 OID 61890)
-- Name: supplier_ingredients supplier_ingredient_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supplier_ingredients
    ADD CONSTRAINT supplier_ingredient_pkey PRIMARY KEY (supplier_ingredient_id);


--
-- TOC entry 4897 (class 2606 OID 61892)
-- Name: suppliers suppliers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suppliers
    ADD CONSTRAINT suppliers_pkey PRIMARY KEY (supplier_id);


--
-- TOC entry 4899 (class 2606 OID 61894)
-- Name: users system_users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT system_users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4852 (class 2606 OID 61896)
-- Name: companies unique_account_per_company; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT unique_account_per_company UNIQUE (account_id);


--
-- TOC entry 4901 (class 2606 OID 61898)
-- Name: users unique_account_per_user; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT unique_account_per_user UNIQUE (account_id);


--
-- TOC entry 4836 (class 1259 OID 61899)
-- Name: accounts_email_active_unique; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX accounts_email_active_unique ON public.accounts USING btree (((deleted_at IS NULL))) INCLUDE (deleted_at) WITH (deduplicate_items='true');


--
-- TOC entry 4841 (class 1259 OID 61900)
-- Name: fk_cart_items_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_cart_items_account_id_idx ON public.cart_items USING btree (company_id);


--
-- TOC entry 4842 (class 1259 OID 61901)
-- Name: fk_cart_items_product_batch_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_cart_items_product_batch_id_idx ON public.cart_items USING btree (batch_id);


--
-- TOC entry 4855 (class 1259 OID 61902)
-- Name: fk_employee_tasks_employee_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_employee_tasks_employee_id_idx ON public.employee_tasks USING btree (employee_id);


--
-- TOC entry 4856 (class 1259 OID 61903)
-- Name: fk_employee_tasks_order_item_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_employee_tasks_order_item_id_idx ON public.employee_tasks USING btree (order_item_id);


--
-- TOC entry 4863 (class 1259 OID 61904)
-- Name: fk_favorites_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_favorites_account_id_idx ON public.favorites USING btree (company_id);


--
-- TOC entry 4864 (class 1259 OID 61905)
-- Name: fk_favorites_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_favorites_product_id_idx ON public.favorites USING btree (product_id);


--
-- TOC entry 4870 (class 1259 OID 61906)
-- Name: fk_order_items_order_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_order_items_order_id_idx ON public.order_items USING btree (order_id);


--
-- TOC entry 4871 (class 1259 OID 61907)
-- Name: fk_order_items_product_batch_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_order_items_product_batch_id_idx ON public.order_items USING btree (batch_id);


--
-- TOC entry 4874 (class 1259 OID 61908)
-- Name: fk_orders_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_orders_account_id_idx ON public.orders USING btree (company_id);


--
-- TOC entry 4877 (class 1259 OID 61909)
-- Name: fk_product_batches_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_product_batches_product_id_idx ON public.product_batches USING btree (product_id);


--
-- TOC entry 4882 (class 1259 OID 61910)
-- Name: fk_product_images_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_product_images_product_id_idx ON public.product_images USING btree (product_id);


--
-- TOC entry 4885 (class 1259 OID 61911)
-- Name: fk_products_category_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_products_category_id_idx ON public.products USING btree (category_id);


--
-- TOC entry 4888 (class 1259 OID 61912)
-- Name: fk_recipe_ingredient_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_recipe_ingredient_id_idx ON public.recipes USING btree (ingredient_id);


--
-- TOC entry 4889 (class 1259 OID 61913)
-- Name: fk_recipe_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_recipe_product_id_idx ON public.recipes USING btree (product_id);


--
-- TOC entry 4857 (class 1259 OID 61914)
-- Name: idx_employee_tasks_employee_end; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_employee_tasks_employee_end ON public.employee_tasks USING btree (employee_id, end_time);


--
-- TOC entry 4858 (class 1259 OID 61915)
-- Name: idx_employee_tasks_employee_start; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_employee_tasks_employee_start ON public.employee_tasks USING btree (employee_id, start_time);


--
-- TOC entry 4865 (class 1259 OID 61916)
-- Name: idx_ingredient_batches_supplier_ingredient; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_ingredient_batches_supplier_ingredient ON public.ingredient_batches USING btree (supplier_ingredient_id);


--
-- TOC entry 4902 (class 1259 OID 62042)
-- Name: idx_reservations_ingredient_batch; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_reservations_ingredient_batch ON public.order_item_ingredient_reservations USING btree (ingredient_batch_id);


--
-- TOC entry 4903 (class 1259 OID 62043)
-- Name: idx_reservations_order_item; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_reservations_order_item ON public.order_item_ingredient_reservations USING btree (order_item_id);


--
-- TOC entry 4892 (class 1259 OID 61917)
-- Name: idx_supplier_ingredient_ingredient; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_supplier_ingredient_ingredient ON public.supplier_ingredients USING btree (ingredient_id);


--
-- TOC entry 4893 (class 1259 OID 61918)
-- Name: idx_supplier_ingredient_supplier; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_supplier_ingredient_supplier ON public.supplier_ingredients USING btree (supplier_id);


--
-- TOC entry 4908 (class 2606 OID 61919)
-- Name: companies companies_account_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_account_id_fkey FOREIGN KEY (account_id) REFERENCES public.accounts(account_id) ON DELETE CASCADE;


--
-- TOC entry 4906 (class 2606 OID 61924)
-- Name: cart_items fk_cart_items_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT fk_cart_items_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4907 (class 2606 OID 61929)
-- Name: cart_items fk_cart_items_product_batch_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT fk_cart_items_product_batch_id FOREIGN KEY (batch_id) REFERENCES public.product_batches(product_batch_id);


--
-- TOC entry 4909 (class 2606 OID 61934)
-- Name: employee_tasks fk_employee_tasks_employee_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT fk_employee_tasks_employee_id FOREIGN KEY (employee_id) REFERENCES public.employees(employee_id);


--
-- TOC entry 4910 (class 2606 OID 61939)
-- Name: employee_tasks fk_employee_tasks_order_item_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT fk_employee_tasks_order_item_id FOREIGN KEY (order_item_id) REFERENCES public.order_items(order_item_id);


--
-- TOC entry 4911 (class 2606 OID 61944)
-- Name: favorites fk_favourites_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT fk_favourites_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4912 (class 2606 OID 61949)
-- Name: favorites fk_favourites_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT fk_favourites_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4913 (class 2606 OID 61954)
-- Name: ingredient_batches fk_ingredient_batches_supplier_ingredient; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches
    ADD CONSTRAINT fk_ingredient_batches_supplier_ingredient FOREIGN KEY (supplier_ingredient_id) REFERENCES public.supplier_ingredients(supplier_ingredient_id);


--
-- TOC entry 4914 (class 2606 OID 61959)
-- Name: order_items fk_order_items_order_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT fk_order_items_order_id FOREIGN KEY (order_id) REFERENCES public.orders(order_id);


--
-- TOC entry 4915 (class 2606 OID 61964)
-- Name: order_items fk_order_items_product_batch_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT fk_order_items_product_batch_id FOREIGN KEY (batch_id) REFERENCES public.product_batches(product_batch_id);


--
-- TOC entry 4916 (class 2606 OID 61969)
-- Name: orders fk_orders_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT fk_orders_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4917 (class 2606 OID 61974)
-- Name: product_batches fk_product_batches_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches
    ADD CONSTRAINT fk_product_batches_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4918 (class 2606 OID 61979)
-- Name: product_images fk_product_images_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images
    ADD CONSTRAINT fk_product_images_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4919 (class 2606 OID 61984)
-- Name: products fk_products_category_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT fk_products_category_id FOREIGN KEY (category_id) REFERENCES public.product_categories(product_category_id);


--
-- TOC entry 4920 (class 2606 OID 61989)
-- Name: recipes fk_recipe_ingredient_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_recipe_ingredient_id FOREIGN KEY (ingredient_id) REFERENCES public.ingredients(ingredient_id);


--
-- TOC entry 4921 (class 2606 OID 61994)
-- Name: recipes fk_recipe_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_recipe_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4925 (class 2606 OID 62032)
-- Name: order_item_ingredient_reservations fk_reservation_ingredient_batch; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_item_ingredient_reservations
    ADD CONSTRAINT fk_reservation_ingredient_batch FOREIGN KEY (ingredient_batch_id) REFERENCES public.ingredient_batches(ingredient_batch_id) ON DELETE RESTRICT;


--
-- TOC entry 4926 (class 2606 OID 62037)
-- Name: order_item_ingredient_reservations fk_reservation_order_item; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_item_ingredient_reservations
    ADD CONSTRAINT fk_reservation_order_item FOREIGN KEY (order_item_id) REFERENCES public.order_items(order_item_id) ON DELETE CASCADE;


--
-- TOC entry 4922 (class 2606 OID 61999)
-- Name: supplier_ingredients fk_supplier_ingredient_ingredient; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supplier_ingredients
    ADD CONSTRAINT fk_supplier_ingredient_ingredient FOREIGN KEY (ingredient_id) REFERENCES public.ingredients(ingredient_id);


--
-- TOC entry 4923 (class 2606 OID 62004)
-- Name: supplier_ingredients fk_supplier_ingredient_supplier; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supplier_ingredients
    ADD CONSTRAINT fk_supplier_ingredient_supplier FOREIGN KEY (supplier_id) REFERENCES public.suppliers(supplier_id);


--
-- TOC entry 4924 (class 2606 OID 62009)
-- Name: users system_users_account_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT system_users_account_id_fkey FOREIGN KEY (account_id) REFERENCES public.accounts(account_id) ON DELETE CASCADE;


-- Completed on 2026-05-22 05:37:37

--
-- PostgreSQL database dump complete
--

\unrestrict cU1fjRbGQ7MSh40SPT1dmoNGrSwo5jZCD1lwvSZtNuPlb3tqMnfYEamJPXAmvrr

