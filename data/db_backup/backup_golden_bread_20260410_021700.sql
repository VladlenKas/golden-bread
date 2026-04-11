--
-- PostgreSQL database dump
--

\restrict FAQjKeWNu4R0hT94S2N1ltoOiCGeWTcJi4vtbdyA5f1YhUw2iuv5pLaSZjXqwki

-- Dumped from database version 17.6
-- Dumped by pg_dump version 17.6

-- Started on 2026-04-10 02:17:11

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
-- TOC entry 921 (class 1247 OID 17144)
-- Name: account_type; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.account_type AS ENUM (
    'user',
    'company'
);


ALTER TYPE public.account_type OWNER TO postgres;

--
-- TOC entry 882 (class 1247 OID 16858)
-- Name: ingredient_batch_status; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.ingredient_batch_status AS ENUM (
    'available',
    'expired',
    'out_of_stock'
);


ALTER TYPE public.ingredient_batch_status OWNER TO postgres;

--
-- TOC entry 891 (class 1247 OID 16882)
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
-- TOC entry 885 (class 1247 OID 16866)
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
-- TOC entry 888 (class 1247 OID 16876)
-- Name: user_role; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.user_role AS ENUM (
    'manager_production',
    'admin'
);


ALTER TYPE public.user_role OWNER TO postgres;

--
-- TOC entry 933 (class 1247 OID 17486)
-- Name: verification_status; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.verification_status AS ENUM (
    'pending',
    'approved',
    'rejected',
    'suspended'
);


ALTER TYPE public.verification_status OWNER TO postgres;

--
-- TOC entry 253 (class 1255 OID 43910)
-- Name: update_batch_production_minutes(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.update_batch_production_minutes() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    NEW.production_minutes_per_batch := (
        SELECT production_time_minutes * NEW.quantity_per_batch
        FROM products
        WHERE product_id = NEW.product_id
    );
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.update_batch_production_minutes() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 244 (class 1259 OID 35479)
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
-- TOC entry 243 (class 1259 OID 35478)
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
-- TOC entry 5101 (class 0 OID 0)
-- Dependencies: 243
-- Name: accounts_account_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.accounts_account_id_seq OWNED BY public.accounts.account_id;


--
-- TOC entry 240 (class 1259 OID 17419)
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
-- TOC entry 239 (class 1259 OID 17418)
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
-- TOC entry 5102 (class 0 OID 0)
-- Dependencies: 239
-- Name: cart_items_new_cart_item_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cart_items_new_cart_item_id_seq OWNED BY public.cart_items.cart_item_id;


--
-- TOC entry 248 (class 1259 OID 35507)
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
-- TOC entry 247 (class 1259 OID 35506)
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
-- TOC entry 5103 (class 0 OID 0)
-- Dependencies: 247
-- Name: companies_company_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.companies_company_id_seq OWNED BY public.companies.company_id;


--
-- TOC entry 249 (class 1259 OID 43935)
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
-- TOC entry 250 (class 1259 OID 43955)
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
-- TOC entry 5104 (class 0 OID 0)
-- Dependencies: 250
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
    deleted_at timestamp with time zone
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
-- TOC entry 5105 (class 0 OID 0)
-- Dependencies: 217
-- Name: employees_employee_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employees_employee_id_seq OWNED BY public.employees.employee_id;


--
-- TOC entry 236 (class 1259 OID 17092)
-- Name: favorites; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.favorites (
    favorite_id integer NOT NULL,
    product_id integer NOT NULL,
    company_id integer
);


ALTER TABLE public.favorites OWNER TO postgres;

--
-- TOC entry 235 (class 1259 OID 17091)
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
-- TOC entry 5106 (class 0 OID 0)
-- Dependencies: 235
-- Name: favourites_favourite_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.favourites_favourite_id_seq OWNED BY public.favorites.favorite_id;


--
-- TOC entry 226 (class 1259 OID 16945)
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
-- TOC entry 5107 (class 0 OID 0)
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
    name character varying(100) NOT NULL
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
-- TOC entry 5108 (class 0 OID 0)
-- Dependencies: 223
-- Name: ingredients_ingredient_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredients_ingredient_id_seq OWNED BY public.ingredients.ingredient_id;


--
-- TOC entry 238 (class 1259 OID 17399)
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
-- TOC entry 237 (class 1259 OID 17398)
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
-- TOC entry 5109 (class 0 OID 0)
-- Dependencies: 237
-- Name: order_items_new_order_item_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.order_items_new_order_item_id_seq OWNED BY public.order_items.order_item_id;


--
-- TOC entry 234 (class 1259 OID 17028)
-- Name: orders; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.orders (
    order_id integer NOT NULL,
    status public.order_status NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    company_id integer NOT NULL,
    canceled_at timestamp with time zone,
    cancel_reason text
);


ALTER TABLE public.orders OWNER TO postgres;

--
-- TOC entry 233 (class 1259 OID 17027)
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
-- TOC entry 5110 (class 0 OID 0)
-- Dependencies: 233
-- Name: orders_order_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.orders_order_id_seq OWNED BY public.orders.order_id;


--
-- TOC entry 242 (class 1259 OID 17462)
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
-- TOC entry 241 (class 1259 OID 17461)
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
-- TOC entry 5111 (class 0 OID 0)
-- Dependencies: 241
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
    color character varying(6) NOT NULL,
    deleted_at timestamp with time zone
);


ALTER TABLE public.product_categories OWNER TO postgres;

--
-- TOC entry 230 (class 1259 OID 16974)
-- Name: product_images; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product_images (
    product_image_id integer NOT NULL,
    product_id integer NOT NULL,
    image_path character varying
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
-- TOC entry 5112 (class 0 OID 0)
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
-- TOC entry 5113 (class 0 OID 0)
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
-- TOC entry 5114 (class 0 OID 0)
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
-- TOC entry 5115 (class 0 OID 0)
-- Dependencies: 231
-- Name: recipes_recipe_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.recipes_recipe_id_seq OWNED BY public.recipes.recipe_id;


--
-- TOC entry 252 (class 1259 OID 52142)
-- Name: supplier_ingredients; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.supplier_ingredients (
    supplier_ingredient_id integer NOT NULL,
    supplier_id integer NOT NULL,
    ingredient_id integer NOT NULL,
    name character varying(100) NOT NULL,
    price numeric(10,2) NOT NULL,
    unit public.ingredient_unit NOT NULL,
    weight numeric(10,3) NOT NULL,
    shelf_life_months integer NOT NULL,
    deleted_at timestamp with time zone
);


ALTER TABLE public.supplier_ingredients OWNER TO postgres;

--
-- TOC entry 251 (class 1259 OID 52141)
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
-- TOC entry 5116 (class 0 OID 0)
-- Dependencies: 251
-- Name: supplier_ingredient_supplier_ingredient_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.supplier_ingredient_supplier_ingredient_id_seq OWNED BY public.supplier_ingredients.supplier_ingredient_id;


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
    deleted_at timestamp with time zone
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
-- TOC entry 5117 (class 0 OID 0)
-- Dependencies: 221
-- Name: suppliers_supplier_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.suppliers_supplier_id_seq OWNED BY public.suppliers.supplier_id;


--
-- TOC entry 246 (class 1259 OID 35493)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    user_id integer NOT NULL,
    account_id integer NOT NULL,
    role public.user_role NOT NULL,
    firstname character varying(50) NOT NULL,
    lastname character varying(50) NOT NULL,
    patronymic character varying(50),
    birthday date NOT NULL
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 245 (class 1259 OID 35492)
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
-- TOC entry 5118 (class 0 OID 0)
-- Dependencies: 245
-- Name: system_users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.system_users_user_id_seq OWNED BY public.users.user_id;


--
-- TOC entry 4818 (class 2604 OID 35482)
-- Name: accounts account_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts ALTER COLUMN account_id SET DEFAULT nextval('public.accounts_account_id_seq'::regclass);


--
-- TOC entry 4815 (class 2604 OID 17422)
-- Name: cart_items cart_item_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items ALTER COLUMN cart_item_id SET DEFAULT nextval('public.cart_items_new_cart_item_id_seq'::regclass);


--
-- TOC entry 4822 (class 2604 OID 35510)
-- Name: companies company_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies ALTER COLUMN company_id SET DEFAULT nextval('public.companies_company_id_seq'::regclass);


--
-- TOC entry 4823 (class 2604 OID 43956)
-- Name: employee_tasks employee_task_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks ALTER COLUMN employee_task_id SET DEFAULT nextval('public.employee_tasks_new_employee_task_id_seq'::regclass);


--
-- TOC entry 4799 (class 2604 OID 16907)
-- Name: employees employee_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees ALTER COLUMN employee_id SET DEFAULT nextval('public.employees_employee_id_seq'::regclass);


--
-- TOC entry 4812 (class 2604 OID 17095)
-- Name: favorites favorite_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites ALTER COLUMN favorite_id SET DEFAULT nextval('public.favourites_favourite_id_seq'::regclass);


--
-- TOC entry 4803 (class 2604 OID 16948)
-- Name: ingredient_batches ingredient_batch_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches ALTER COLUMN ingredient_batch_id SET DEFAULT nextval('public.ingredient_batches_ingredient_batch_id_seq'::regclass);


--
-- TOC entry 4802 (class 2604 OID 16932)
-- Name: ingredients ingredient_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients ALTER COLUMN ingredient_id SET DEFAULT nextval('public.ingredients_ingredient_id_seq'::regclass);


--
-- TOC entry 4813 (class 2604 OID 17402)
-- Name: order_items order_item_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items ALTER COLUMN order_item_id SET DEFAULT nextval('public.order_items_new_order_item_id_seq'::regclass);


--
-- TOC entry 4810 (class 2604 OID 17031)
-- Name: orders order_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders ALTER COLUMN order_id SET DEFAULT nextval('public.orders_order_id_seq'::regclass);


--
-- TOC entry 4816 (class 2604 OID 17465)
-- Name: product_batches product_batch_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches ALTER COLUMN product_batch_id SET DEFAULT nextval('public.product_batches_new_product_batch_id_seq'::regclass);


--
-- TOC entry 4800 (class 2604 OID 16915)
-- Name: product_categories product_category_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_categories ALTER COLUMN product_category_id SET DEFAULT nextval('public.production_categories_production_category_id_seq'::regclass);


--
-- TOC entry 4808 (class 2604 OID 16977)
-- Name: product_images product_image_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images ALTER COLUMN product_image_id SET DEFAULT nextval('public.product_images_product_image_id_seq'::regclass);


--
-- TOC entry 4804 (class 2604 OID 16961)
-- Name: products product_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products ALTER COLUMN product_id SET DEFAULT nextval('public.products_product_id_seq'::regclass);


--
-- TOC entry 4809 (class 2604 OID 16993)
-- Name: recipes recipe_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes ALTER COLUMN recipe_id SET DEFAULT nextval('public.recipes_recipe_id_seq'::regclass);


--
-- TOC entry 4825 (class 2604 OID 52145)
-- Name: supplier_ingredients supplier_ingredient_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supplier_ingredients ALTER COLUMN supplier_ingredient_id SET DEFAULT nextval('public.supplier_ingredient_supplier_ingredient_id_seq'::regclass);


--
-- TOC entry 4801 (class 2604 OID 16922)
-- Name: suppliers supplier_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suppliers ALTER COLUMN supplier_id SET DEFAULT nextval('public.suppliers_supplier_id_seq'::regclass);


--
-- TOC entry 4821 (class 2604 OID 35496)
-- Name: users user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.system_users_user_id_seq'::regclass);


--
-- TOC entry 5087 (class 0 OID 35479)
-- Dependencies: 244
-- Data for Name: accounts; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.accounts (account_id, email, password_hash, account_type, verification_status, session, session_expires_at, deleted_at, created_at) FROM stdin;
17	fff	fff	user	approved	\N	\N	\N	2026-03-26 20:24:36.814412+05
43	asdfasdfasdf@gmail.com	$2a$11$5G5CbJeOJfo9VoV0RjaRb.gnKbmr5NFaysaaM0njCc1braR5fJ74C	company	pending	216b9e1542784cbdb5212e6cf175f133	2026-02-19 09:44:24.921468+05	\N	2026-03-26 20:24:36.814412+05
42	ggg@gmail.com	$2a$11$ZRcqbgbxne6QuLTFkt6pNu8J/tGxhURdqpoUJTdBCh1d3nyKOnGLm	company	approved	5c1eec5347c74b07aea05643ff3a674d	2026-03-28 00:13:13.570735+05	\N	2026-03-26 20:24:36.814412+05
20	vlada@gmail.com	$2a$11$7CSjSaipq0Xe/Q4IwZ0tLuG3pHt9y24eJbQ.3mt2WQmUtwlWJG77.	company	pending	2a610b58-bef8-4000-aa22-52c635403e54@2026-01-29T03:47:22.9490343Z	2026-02-05 08:47:22.950119+05	\N	2026-03-26 20:24:36.814412+05
21	adfas@afd.re	$2a$11$xHxEyLw9QSq4dR.coJdIrOQCcWLs6JllEutEjB6UwNvT7.zzfS5e.	company	pending	1b11277a-48a3-4ca7-a958-7745589b0411@2026-01-29T03:52:02.1261123Z	2026-02-05 08:52:02.12612+05	\N	2026-03-26 20:24:36.814412+05
22	aaa@gmail.com	$2a$11$eptuAz/j8macxALQjWmH2O3k3lCL0KdUzqda0Aeo5h5Zk6DKBdC7u	company	pending	9285bffa-8cb5-4ebc-9047-e46a3bd59d90@2026-01-29T04:01:51.5296422Z	2026-02-05 09:01:51.52965+05	\N	2026-03-26 20:24:36.814412+05
23	adfas@FDSA.RU	$2a$11$yaq4XNv5W/UN.U/jaISkTe5bH59tdLLIs0BZy4O7a6./2uM/TKiD.	company	pending	1ef9d02b-a51b-45a9-9292-8602d6c60afd@2026-01-29T04:26:49.0933499Z	2026-02-05 09:26:49.094528+05	\N	2026-03-26 20:24:36.814412+05
24	adfas@sdfgsd.rt	$2a$11$9JNcfdkf8qi./TaX.G46oe3Vk79D5KR/bUJ3Sp.x4sMT1S./4qGwu	company	pending	a8053436-a87b-4509-94de-99a76d4ca361@2026-01-29T04:31:11.7163860Z	2026-02-05 09:31:11.716615+05	\N	2026-03-26 20:24:36.814412+05
25	vlad@gmail.coq	$2a$11$LzG8Fk68K5QMjz7yHfgA.uiy1VmlTS3hxzueqeptgAEUblvdr8Itq	company	pending	f7c81dbf-64ae-4e82-ae92-c9f79f4ac773@2026-01-30T20:30:53.7770831Z	2026-02-07 01:30:53.777118+05	\N	2026-03-26 20:24:36.814412+05
26	goldenbread@gmail.com	$2a$11$00/MpdNQvF6alBLjtOzsqe5YHIVIgD4m755xZLo5QkgqqO1U1B7ge	company	pending	ea1ec350-f32a-4330-a9b8-fa2d2c766ebc@2026-01-30T22:52:30.3770937Z	2026-02-07 03:52:30.377134+05	\N	2026-03-26 20:24:36.814412+05
28	fasf@asfa.re	$2a$11$DdZ0eHjzbVgS221wg6B0R.wjotYD00b16SH7krDwXC0haGzVEig/G	company	pending	tiny-bear-494	2026-02-09 06:16:27.197078+05	\N	2026-03-26 20:24:36.814412+05
29	asdf@gmail.com	$2a$11$0jqH60apwX7QgA44TXCly.OT.bMXZFYIzOTF7aEmwoh/J21/2rTLG	company	pending	fluffy-cat-233	2026-02-11 00:25:55.699158+05	\N	2026-03-26 20:24:36.814412+05
32	asdfasdf@gmail.com	$2a$11$Ji083gNaqTaogIKVxrU8W.L1ZWcL4nFCgMDJqAUu/9wejbiV0pEdS	company	pending	cozy-owl-130	2026-02-11 21:40:32.283678+05	\N	2026-03-26 20:24:36.814412+05
33	asdfasdasdf@gmail.com	$2a$11$6/AT26wCko/qRye4OxDoBOoKU3700vn/RjutODUnkgs447uM.D4V.	company	pending	cute-dog-330	2026-02-11 21:45:11.171613+05	\N	2026-03-26 20:24:36.814412+05
34	asdfasdasdsf@gmail.com	$2a$11$fv0zZa0Sex1d.q6xHFdtH.u8HZNoNQuqW3xdUrWxewBg1uaGkjgEW	company	pending	soft-owl-200	2026-02-11 21:45:36.955872+05	\N	2026-03-26 20:24:36.814412+05
35	asdfasadasdsf@gmail.com	$2a$11$hgEHcncXmzFfnjakXlGeCOJ5uGrnwzQesZ9.KqEzxu5lwH3U6YaN.	company	pending	soft-owl-777	2026-02-11 21:48:50.327157+05	\N	2026-03-26 20:24:36.814412+05
36	asdfasaddasdsf@gmail.com	$2a$11$t6mjRKf2Qow5qoesCSGXAeyN8H4DQnRVZUSeGnBNi.OTv5OKj9Yke	company	pending	cute-dog-882	2026-02-11 21:49:02.569738+05	\N	2026-03-26 20:24:36.814412+05
37	asdaddasdsf@gmail.com	$2a$11$wzYfaGhaD.zmIwem7vl9p.jwjFC4FJZoQYOZjj7paKE1ihaB9Yozm	company	pending	fluffy-bunny-967	2026-02-11 22:15:42.808389+05	\N	2026-03-26 20:24:36.814412+05
38	asdaddsdsf@gmail.com	$2a$11$UIqaxqEIvOq0XG9gZTtm7uXiQDWGbkW9UvOtSnDWfE5xpIR/W2sPq	company	pending	tiny-fox-770	2026-02-11 22:16:03.115485+05	\N	2026-03-26 20:24:36.814412+05
39	asdf@gmail.co	$2a$11$04S9UNlaoEAcaWYRKJntPu/2iyvq/MNlTXctsm.IBgEwlOnxlNPH6	company	pending	cute-cat-401	2026-02-11 23:04:14.190969+05	\N	2026-03-26 20:24:36.814412+05
44	dfdf@gmail.com	$2a$11$lo2f4p9pp.PBqugcsgjqCOl/XsxoYGs89ZwQ04GDAfZtGUcdi7UH6	company	pending	3c37054a483743eda9e98b28704be6c3	2026-02-19 16:00:26.419652+05	\N	2026-03-26 20:24:36.814412+05
19	vlad@gmail.com	$2a$11$w3NvdjDCT1sx3VLBlXdcTuJLkfNibmAyy0AYqlUdApK85kzHnK8Xe	company	pending	30dfe865200542ebbea8081b54d33a12	2026-02-19 02:17:01.28283+05	\N	2026-03-26 20:24:36.814412+05
40	asdfasdasdff@gmail.com	$2a$11$r4vFOuLhnirZICN7RrW9peon77t29WwaqD6GNY3jPwddhnL97Aex2	company	pending	aa59366497d4433c88ae3232a9f0eec7	2026-02-17 03:16:43.098618+05	\N	2026-03-26 20:24:36.814412+05
16	asdf	asdf	user	pending	\N	\N	2026-02-22 14:15:15.170075+05	2026-03-26 20:24:36.814412+05
18	aaa	$2a$11$LZjN5vfJOhpdB1NcgPNCUu.Ko2EDoVpYrXRLdm3b45M/jLOZ20O1O	user	approved	fec082611205400eb8e8657755d95647	2026-03-24 17:10:52.014421+05	\N	2026-03-26 20:24:36.814412+05
46	test@gmail.com	$2a$11$gk/ehxCTUt0UftTWw2P0a.KNmVzUXP6ua9bgH3X4RaLpTzXbg8aZS	company	pending	2e39dcd8bd99476e9a5c2445e62f8f10	2026-03-27 20:29:17.180378+05	\N	-infinity
47	asdfasdfdfgdfasdf@gmail.com	$2a$11$RzjJka9iy5xk81ZNTbx1tuXpnRF8kVo4BTzXWaeAgBdrDiSlX.OM2	company	pending	6b6adc6916944b64a9093707271109cb	2026-03-27 20:39:42.32531+05	\N	2026-03-26 20:39:42.694344+05
\.


--
-- TOC entry 5083 (class 0 OID 17419)
-- Dependencies: 240
-- Data for Name: cart_items; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.cart_items (cart_item_id, batch_id, quantity, company_id) FROM stdin;
85	7	1	23
82	9	4	23
86	6	1	23
84	13	3	23
\.


--
-- TOC entry 5091 (class 0 OID 35507)
-- Dependencies: 248
-- Data for Name: companies; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.companies (company_id, account_id, name, inn, ogrn, phone, address) FROM stdin;
3	21	jhhj	1234567815	1212121212120	\N	\N
4	22	026912000636sda	1274567812	1212121212123	\N	\N
5	23	026912000636sd	6234567812	1212121212125	\N	\N
6	24	ИП Касимов Владлен Ильшатович	4334567812	1212121212100	\N	\N
7	25	ИП Касимов Владлен Ильшатович asdf	6543467811	0000000000000	\N	\N
8	26	Golden Bread	9018918091	5990818012834	\N	\N
11	29	фваа	1244567812	3456211152355	\N	\N
14	32	фыва	1244567814	1244567812341	\N	\N
15	33	фывааа	5244567834	1244517812341	\N	\N
16	34	фывфааа	1244367834	1254517812341	\N	\N
17	35	фывффааа	2244367834	1214517812341	\N	\N
18	36	фaывффааа	2241366834	1234517812341	\N	\N
19	37	фвффааа	1231414221	1022234567890	\N	\N
20	38	фвффаа	1231414231	1027000001234	\N	\N
21	39	фываа	1212121214	5121231413413	\N	\N
22	40	asdf	1234123412	1234234314122	\N	\N
2	19	fsdfg	1234567812	1212121212122	\N	\N
24	43	asdfasdf	1212121212	1212121212121	\N	\N
25	44	asdfSADFAASDF	1212121285	1212121212520	\N	\N
23	42	Me Company Test Account 2	1212121218	1254556346451	89377888091	12121212
27	46	testtest	0532883891	1133531543143	\N	\N
28	47	asdfsdfsdf	1212125433	1543253244543	\N	\N
\.


--
-- TOC entry 5092 (class 0 OID 43935)
-- Dependencies: 249
-- Data for Name: employee_tasks; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employee_tasks (employee_task_id, employee_id, order_item_id, status, assigned_quantity, completed_quantity, start_time, end_time) FROM stdin;
13	1	13	in_progress	10	0	2026-04-06 08:00:00+05	2026-04-06 09:30:00+05
14	1	14	in_progress	15	0	2026-04-06 10:00:00+05	2026-04-06 11:30:00+05
15	1	15	in_progress	20	0	2026-04-06 13:00:00+05	2026-04-06 14:00:00+05
16	1	16	in_progress	12	0	2026-04-06 15:30:00+05	2026-04-06 16:30:00+05
\.


--
-- TOC entry 5061 (class 0 OID 16904)
-- Dependencies: 218
-- Data for Name: employees; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employees (employee_id, firstname, lastname, patronymic, birthday, deleted_at) FROM stdin;
1	Иван	Петров	Сергеевич	1985-03-15	\N
2	Мария	Сидорова	Александровна	1990-07-22	\N
3	Алексей	Иванов	Викторович	1988-11-10	\N
4	Елена	Кузнецова	\N	1992-05-18	\N
5	Дмитрий	Смирнов	Олегович	1987-09-03	\N
6	Анна	Попова	Ивановна	1991-12-27	\N
7	Сергей	Васильев	\N	1986-02-14	\N
8	Ольга	Морозова	Петровна	1989-08-30	\N
9	Михаил	Новиков	Андреевич	1993-04-12	\N
10	Татьяна	Федорова	\N	1984-10-05	\N
11	Николай	Михайлов	Юрьевич	1990-01-19	\N
12	Ирина	Соколова	Сергеевна	1988-06-25	\N
13	Владимир	Лебедев	\N	1987-12-08	\N
14	Екатерина	Егорова	Дмитриевна	1992-03-02	\N
15	Павел	Козлов	Игоревич	1985-09-17	\N
16	Наталья	Алексеева	\N	1991-07-11	\N
17	Андрей	Зайцев	Михайлович	1989-05-28	\N
18	Светлана	Лебедева	Викторовна	1986-11-20	\N
19	Константин	Быков	\N	1994-02-07	\N
20	Юлия	Громова	Павловна	1990-08-14	\N
\.


--
-- TOC entry 5079 (class 0 OID 17092)
-- Dependencies: 236
-- Data for Name: favorites; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.favorites (favorite_id, product_id, company_id) FROM stdin;
60	11	23
69	7	23
70	6	23
39	4	23
50	9	23
\.


--
-- TOC entry 5069 (class 0 OID 16945)
-- Dependencies: 226
-- Data for Name: ingredient_batches; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredient_batches (ingredient_batch_id, status, purchased_quantity, remaining_quantity, delivery_date, expiry_date, supplier_ingredient_id) FROM stdin;
1	available	100	9.500	2025-12-01	2026-12-01	1
2	available	50	8.000	2025-12-15	2026-01-15	2
3	available	50	8.500	2025-11-20	2027-11-20	3
4	available	25	4.800	2025-10-01	2028-10-01	4
5	available	10	1.500	2025-12-10	2026-03-10	5
6	available	20	9.200	2025-12-01	2026-06-01	7
7	available	5	4.900	2025-11-01	2027-05-01	8
8	available	15	4.500	2025-11-15	2026-11-15	9
9	available	2	1.980	2025-10-20	2027-10-20	10
\.


--
-- TOC entry 5067 (class 0 OID 16929)
-- Dependencies: 224
-- Data for Name: ingredients; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredients (ingredient_id, name) FROM stdin;
1	Мука пшеничная
2	Молоко
3	Сахар
4	Соль
5	Дрожжи свежие
6	Яйца
7	Масло сливочное
8	Корица молотая
9	Изюм
10	Ванилин
11	Мак молотый
12	Яблоки
13	Вишня
14	Говядина фарш
15	Лук репчатый
16	Мука ржаная
17	Кориандр
18	Мед
19	Сметана
20	Сыр сливочный
21	Сахарная пудра
\.


--
-- TOC entry 5081 (class 0 OID 17399)
-- Dependencies: 238
-- Data for Name: order_items; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.order_items (order_item_id, order_id, batch_id, status, quantity, unit_price, units_per_batch) FROM stdin;
13	1	1	completed	120	45.50	20
14	1	2	completed	80	52.00	10
15	2	3	completed	150	47.00	25
16	2	4	completed	60	49.90	15
17	3	5	completed	200	43.75	20
18	4	6	in_progress	180	51.20	30
19	4	7	in_progress	90	46.80	15
20	5	8	completed	110	44.00	10
21	6	9	completed	140	53.10	20
22	6	10	completed	70	48.60	10
\.


--
-- TOC entry 5077 (class 0 OID 17028)
-- Dependencies: 234
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orders (order_id, status, start_date, end_date, created_at, company_id, canceled_at, cancel_reason) FROM stdin;
1	completed	2025-12-01	2025-12-01	2025-12-01 10:00:00+05	18	\N	\N
2	completed	2025-12-05	2025-12-05	2025-12-05 11:30:00+05	18	\N	\N
3	completed	2025-12-10	2025-12-10	2025-12-10 14:20:00+05	18	\N	\N
4	in_progress	2025-12-15	2025-12-15	2025-12-15 09:15:00+05	18	\N	\N
5	completed	2025-11-05	2025-11-05	2025-11-05 12:00:00+05	18	\N	\N
6	completed	2025-11-12	2025-11-12	2025-11-12 15:30:00+05	18	\N	\N
\.


--
-- TOC entry 5085 (class 0 OID 17462)
-- Dependencies: 242
-- Data for Name: product_batches; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_batches (product_batch_id, product_id, quantity_units, markup_percent) FROM stdin;
1	1	10	115
2	2	12	110
3	3	10	120
4	4	5	90
5	5	4	85
6	6	3	95
7	7	8	70
8	8	10	60
9	9	6	75
10	10	2	60
11	11	2	55
12	12	3	65
13	5	10	80
\.


--
-- TOC entry 5063 (class 0 OID 16912)
-- Dependencies: 220
-- Data for Name: product_categories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_categories (product_category_id, name, color, deleted_at) FROM stdin;
1	Булочки	FFD700	\N
2	Пироги	FF6347	\N
3	Хлеб	D2691E	\N
4	Торты	FF69B4	\N
\.


--
-- TOC entry 5073 (class 0 OID 16974)
-- Dependencies: 230
-- Data for Name: product_images; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_images (product_image_id, product_id, image_path) FROM stdin;
1	7	47703a2cca2dfd468c6bf5d0fe7e0e9e.jpg
2	5	Gy8CGhW5pet1nMrgOaiFZWKIbV4s3R77P33zklSN_780.png
3	4	LWntbI2Xq8g77H4UDshBej0rhE8b6i4LxOmAwrP5_780.png
8	5	cherry_pie_2.jpg
9	5	cherry_pie_3.webp
10	5	cherry_pie_4.webp
11	5	cherry_pie_5.jpg
12	5	cherry_pie_6.jpg
13	5	cherry_pie_8.jpg
\.


--
-- TOC entry 5071 (class 0 OID 16958)
-- Dependencies: 228
-- Data for Name: products; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.products (product_id, category_id, name, description, cost_price, weight, production_time_minutes, deleted_at, storage_temp_min, storage_temp_max, shelf_life_days) FROM stdin;
1	1	Булочка с корицей	Ароматная сдобная булочка с корицей и сахаром	25.50	0.080	45	\N	18.0	25.0	2
2	1	Булочка с изюмом	Нежная сдобная булочка с сочным изюмом	28.00	0.090	50	\N	18.0	25.0	2
3	1	Булочка с маком	Воздушная булочка с маковой начинкой	30.00	0.085	55	\N	18.0	25.0	2
4	2	Яблочный пирог	Классический пирог с яблоками и корицей	120.00	1.200	120	\N	2.0	6.0	5
5	2	Вишневый пирог	Сочный пирог с вишневой начинкой	135.00	1.100	130	\N	2.0	6.0	5
6	2	Мясной пирог	Сытный пирог с говядиной и луком	180.00	1.500	150	\N	2.0	6.0	3
7	3	Бородинский	Тёмный ржаной хлеб с кориандром	35.00	0.700	180	\N	18.0	22.0	4
8	3	Батон нарезной	Классический белый батон	25.00	0.400	90	\N	18.0	22.0	4
9	3	Чиабатта	Итальянский хлеб с хрустящей корочкой	45.00	0.350	200	\N	18.0	25.0	1
10	4	Наполеон	Слоёный торт с заварным кремом	450.00	1.800	240	\N	2.0	6.0	3
11	4	Медовик	Медовые коржи с сметанным кремом	380.00	1.500	210	\N	2.0	6.0	5
12	4	Чизкейк	Нежный сырный торт	520.00	1.200	180	\N	-2.0	6.0	5
\.


--
-- TOC entry 5075 (class 0 OID 16990)
-- Dependencies: 232
-- Data for Name: recipes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.recipes (recipe_id, product_id, ingredient_id, quantity) FROM stdin;
103	4	1	0.400
104	4	3	0.150
105	4	4	0.005
106	4	6	2.000
107	4	7	0.150
109	4	8	0.010
110	4	10	0.002
111	5	1	0.350
112	5	3	0.180
113	5	4	0.005
114	5	6	2.000
115	5	7	0.120
117	5	10	0.002
118	6	1	0.400
119	6	4	0.010
120	6	6	1.000
121	6	7	0.100
125	7	1	0.100
126	7	2	0.300
127	7	4	0.015
128	7	5	0.015
130	8	1	0.500
131	8	2	0.300
132	8	3	0.030
133	8	4	0.010
134	8	5	0.015
135	8	7	0.030
136	9	1	0.450
1	1	1	0.300
2	1	2	0.150
3	1	3	0.050
4	1	4	0.005
5	1	5	0.010
6	1	6	1.000
7	1	7	0.050
8	1	8	0.005
9	1	10	0.001
137	9	2	0.350
138	9	4	0.012
139	9	5	0.005
140	9	7	0.040
141	10	1	0.600
142	10	3	0.200
143	10	6	4.000
144	10	7	0.400
145	10	2	0.500
146	10	10	0.003
147	11	1	0.400
148	11	3	0.150
149	11	6	3.000
152	11	10	0.002
174	12	1	0.200
175	12	6	2.000
176	12	7	0.050
177	12	20	0.600
178	12	3	0.150
180	12	10	0.002
108	4	12	0.800
116	5	13	0.700
122	6	14	0.500
123	6	15	0.200
124	7	16	0.400
129	7	17	0.010
150	11	18	0.150
151	11	19	0.400
179	12	3	0.100
181	2	9	0.080
85	2	1	0.300
86	2	2	0.150
87	2	3	0.040
88	2	4	0.005
89	2	5	0.010
90	2	6	1.000
91	2	7	0.040
92	2	9	0.060
93	2	10	0.001
94	3	1	0.280
95	3	2	0.140
96	3	3	0.050
97	3	4	0.005
98	3	5	0.010
99	3	6	1.000
100	3	7	0.050
101	3	12	0.030
102	3	10	0.001
\.


--
-- TOC entry 5095 (class 0 OID 52142)
-- Dependencies: 252
-- Data for Name: supplier_ingredients; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.supplier_ingredients (supplier_ingredient_id, supplier_id, ingredient_id, name, price, unit, weight, shelf_life_months, deleted_at) FROM stdin;
1	1	1	Мука пшеничная	45.00	kg	1.000	12	\N
2	1	2	Молоко	65.00	l	1.000	1	\N
3	1	3	Сахар	50.00	kg	1.000	24	\N
4	1	4	Соль	15.00	kg	1.000	36	\N
5	1	5	Дрожжи свежие	120.00	kg	0.500	3	\N
6	1	6	Яйца	90.00	pcs	0.060	1	\N
7	1	7	Масло сливочное	550.00	kg	1.000	6	\N
8	1	8	Корица молотая	350.00	kg	0.100	18	\N
9	1	9	Изюм	280.00	kg	1.000	12	\N
10	1	10	Ванилин	450.00	kg	0.050	24	\N
11	1	11	Мак молотый	420.00	kg	1.000	18	\N
12	1	12	Яблоки	120.00	kg	1.000	2	\N
13	1	13	Вишня	180.00	kg	1.000	6	\N
14	1	14	Говядина фарш	450.00	kg	1.000	3	\N
15	1	15	Лук репчатый	40.00	kg	1.000	3	\N
16	1	16	Мука ржаная	55.00	kg	1.000	12	\N
17	1	17	Кориандр	280.00	kg	0.100	24	\N
18	1	18	Мед	350.00	kg	1.000	36	\N
19	1	19	Сметана	180.00	kg	1.000	2	\N
20	1	20	Сыр сливочный	650.00	kg	1.000	2	\N
21	1	21	Сахарная пудра	80.00	kg	1.000	24	\N
\.


--
-- TOC entry 5065 (class 0 OID 16919)
-- Dependencies: 222
-- Data for Name: suppliers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.suppliers (supplier_id, name, email, phone, address, deleted_at) FROM stdin;
1	ООО "ПродИнгредиент"	info@prodingredient.ru	79171234567	г. Уфа, ул. Складская, 15	\N
\.


--
-- TOC entry 5089 (class 0 OID 35493)
-- Dependencies: 246
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (user_id, account_id, role, firstname, lastname, patronymic, birthday) FROM stdin;
1	16	admin	asdf	asdf		2020-01-01
2	17	manager_production	фываф	фаф	Антонович	2006-01-01
3	18	admin	Владлен	Касимов	Ильшатович	2006-01-08
\.


--
-- TOC entry 5119 (class 0 OID 0)
-- Dependencies: 243
-- Name: accounts_account_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.accounts_account_id_seq', 47, true);


--
-- TOC entry 5120 (class 0 OID 0)
-- Dependencies: 239
-- Name: cart_items_new_cart_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.cart_items_new_cart_item_id_seq', 86, true);


--
-- TOC entry 5121 (class 0 OID 0)
-- Dependencies: 247
-- Name: companies_company_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.companies_company_id_seq', 28, true);


--
-- TOC entry 5122 (class 0 OID 0)
-- Dependencies: 250
-- Name: employee_tasks_new_employee_task_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employee_tasks_new_employee_task_id_seq', 16, true);


--
-- TOC entry 5123 (class 0 OID 0)
-- Dependencies: 217
-- Name: employees_employee_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employees_employee_id_seq', 20, true);


--
-- TOC entry 5124 (class 0 OID 0)
-- Dependencies: 235
-- Name: favourites_favourite_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.favourites_favourite_id_seq', 74, true);


--
-- TOC entry 5125 (class 0 OID 0)
-- Dependencies: 225
-- Name: ingredient_batches_ingredient_batch_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredient_batches_ingredient_batch_id_seq', 9, true);


--
-- TOC entry 5126 (class 0 OID 0)
-- Dependencies: 223
-- Name: ingredients_ingredient_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredients_ingredient_id_seq', 21, true);


--
-- TOC entry 5127 (class 0 OID 0)
-- Dependencies: 237
-- Name: order_items_new_order_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.order_items_new_order_item_id_seq', 22, true);


--
-- TOC entry 5128 (class 0 OID 0)
-- Dependencies: 233
-- Name: orders_order_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orders_order_id_seq', 6, true);


--
-- TOC entry 5129 (class 0 OID 0)
-- Dependencies: 241
-- Name: product_batches_new_product_batch_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.product_batches_new_product_batch_id_seq', 13, true);


--
-- TOC entry 5130 (class 0 OID 0)
-- Dependencies: 229
-- Name: product_images_product_image_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.product_images_product_image_id_seq', 13, true);


--
-- TOC entry 5131 (class 0 OID 0)
-- Dependencies: 219
-- Name: production_categories_production_category_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.production_categories_production_category_id_seq', 4, true);


--
-- TOC entry 5132 (class 0 OID 0)
-- Dependencies: 227
-- Name: products_product_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.products_product_id_seq', 12, true);


--
-- TOC entry 5133 (class 0 OID 0)
-- Dependencies: 231
-- Name: recipes_recipe_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.recipes_recipe_id_seq', 180, true);


--
-- TOC entry 5134 (class 0 OID 0)
-- Dependencies: 251
-- Name: supplier_ingredient_supplier_ingredient_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.supplier_ingredient_supplier_ingredient_id_seq', 21, true);


--
-- TOC entry 5135 (class 0 OID 0)
-- Dependencies: 221
-- Name: suppliers_supplier_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.suppliers_supplier_id_seq', 1, true);


--
-- TOC entry 5136 (class 0 OID 0)
-- Dependencies: 245
-- Name: system_users_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.system_users_user_id_seq', 3, true);


--
-- TOC entry 4870 (class 2606 OID 35489)
-- Name: accounts accounts_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts
    ADD CONSTRAINT accounts_pkey PRIMARY KEY (account_id);


--
-- TOC entry 4862 (class 2606 OID 17425)
-- Name: cart_items cart_items_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT cart_items_new_pkey PRIMARY KEY (cart_item_id);


--
-- TOC entry 4876 (class 2606 OID 35554)
-- Name: companies companies_inn_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_inn_key UNIQUE (inn);


--
-- TOC entry 4878 (class 2606 OID 35552)
-- Name: companies companies_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_name_key UNIQUE (name);


--
-- TOC entry 4880 (class 2606 OID 35518)
-- Name: companies companies_ogrn_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_ogrn_key UNIQUE (ogrn);


--
-- TOC entry 4882 (class 2606 OID 35514)
-- Name: companies companies_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_pkey PRIMARY KEY (company_id);


--
-- TOC entry 4886 (class 2606 OID 43942)
-- Name: employee_tasks employee_tasks_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT employee_tasks_new_pkey PRIMARY KEY (employee_task_id);


--
-- TOC entry 4830 (class 2606 OID 16910)
-- Name: employees employees_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (employee_id);


--
-- TOC entry 4854 (class 2606 OID 17098)
-- Name: favorites favourites_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT favourites_pkey PRIMARY KEY (favorite_id);


--
-- TOC entry 4839 (class 2606 OID 16950)
-- Name: ingredient_batches ingredient_batches_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches
    ADD CONSTRAINT ingredient_batches_pkey PRIMARY KEY (ingredient_batch_id);


--
-- TOC entry 4836 (class 2606 OID 16937)
-- Name: ingredients ingredients_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients
    ADD CONSTRAINT ingredients_pkey PRIMARY KEY (ingredient_id);


--
-- TOC entry 4860 (class 2606 OID 17405)
-- Name: order_items order_items_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT order_items_new_pkey PRIMARY KEY (order_item_id);


--
-- TOC entry 4852 (class 2606 OID 17033)
-- Name: orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (order_id);


--
-- TOC entry 4867 (class 2606 OID 17468)
-- Name: product_batches product_batches_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches
    ADD CONSTRAINT product_batches_new_pkey PRIMARY KEY (product_batch_id);


--
-- TOC entry 4845 (class 2606 OID 16982)
-- Name: product_images product_images_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images
    ADD CONSTRAINT product_images_pkey PRIMARY KEY (product_image_id);


--
-- TOC entry 4832 (class 2606 OID 16917)
-- Name: product_categories production_categories_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_categories
    ADD CONSTRAINT production_categories_pkey PRIMARY KEY (product_category_id);


--
-- TOC entry 4842 (class 2606 OID 16966)
-- Name: products products_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT products_pkey PRIMARY KEY (product_id);


--
-- TOC entry 4849 (class 2606 OID 16995)
-- Name: recipes recipes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT recipes_pkey PRIMARY KEY (recipe_id);


--
-- TOC entry 4894 (class 2606 OID 52147)
-- Name: supplier_ingredients supplier_ingredient_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supplier_ingredients
    ADD CONSTRAINT supplier_ingredient_pkey PRIMARY KEY (supplier_ingredient_id);


--
-- TOC entry 4834 (class 2606 OID 16927)
-- Name: suppliers suppliers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suppliers
    ADD CONSTRAINT suppliers_pkey PRIMARY KEY (supplier_id);


--
-- TOC entry 4872 (class 2606 OID 35498)
-- Name: users system_users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT system_users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4884 (class 2606 OID 35520)
-- Name: companies unique_account_per_company; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT unique_account_per_company UNIQUE (account_id);


--
-- TOC entry 4874 (class 2606 OID 35500)
-- Name: users unique_account_per_user; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT unique_account_per_user UNIQUE (account_id);


--
-- TOC entry 4868 (class 1259 OID 43743)
-- Name: accounts_email_active_unique; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX accounts_email_active_unique ON public.accounts USING btree (((deleted_at IS NULL))) INCLUDE (deleted_at) WITH (deduplicate_items='true');


--
-- TOC entry 4863 (class 1259 OID 35535)
-- Name: fk_cart_items_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_cart_items_account_id_idx ON public.cart_items USING btree (company_id);


--
-- TOC entry 4864 (class 1259 OID 17437)
-- Name: fk_cart_items_product_batch_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_cart_items_product_batch_id_idx ON public.cart_items USING btree (batch_id);


--
-- TOC entry 4887 (class 1259 OID 43943)
-- Name: fk_employee_tasks_employee_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_employee_tasks_employee_id_idx ON public.employee_tasks USING btree (employee_id);


--
-- TOC entry 4888 (class 1259 OID 43944)
-- Name: fk_employee_tasks_order_item_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_employee_tasks_order_item_id_idx ON public.employee_tasks USING btree (order_item_id);


--
-- TOC entry 4855 (class 1259 OID 35541)
-- Name: fk_favorites_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_favorites_account_id_idx ON public.favorites USING btree (company_id);


--
-- TOC entry 4856 (class 1259 OID 17110)
-- Name: fk_favorites_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_favorites_product_id_idx ON public.favorites USING btree (product_id);


--
-- TOC entry 4857 (class 1259 OID 17416)
-- Name: fk_order_items_order_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_order_items_order_id_idx ON public.order_items USING btree (order_id);


--
-- TOC entry 4858 (class 1259 OID 17417)
-- Name: fk_order_items_product_batch_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_order_items_product_batch_id_idx ON public.order_items USING btree (batch_id);


--
-- TOC entry 4850 (class 1259 OID 35547)
-- Name: fk_orders_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_orders_account_id_idx ON public.orders USING btree (company_id);


--
-- TOC entry 4865 (class 1259 OID 17474)
-- Name: fk_product_batches_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_product_batches_product_id_idx ON public.product_batches USING btree (product_id);


--
-- TOC entry 4843 (class 1259 OID 16988)
-- Name: fk_product_images_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_product_images_product_id_idx ON public.product_images USING btree (product_id);


--
-- TOC entry 4840 (class 1259 OID 16972)
-- Name: fk_products_category_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_products_category_id_idx ON public.products USING btree (category_id);


--
-- TOC entry 4846 (class 1259 OID 17007)
-- Name: fk_recipe_ingredient_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_recipe_ingredient_id_idx ON public.recipes USING btree (ingredient_id);


--
-- TOC entry 4847 (class 1259 OID 17006)
-- Name: fk_recipe_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_recipe_product_id_idx ON public.recipes USING btree (product_id);


--
-- TOC entry 4889 (class 1259 OID 52139)
-- Name: idx_employee_tasks_employee_end; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_employee_tasks_employee_end ON public.employee_tasks USING btree (employee_id, end_time);


--
-- TOC entry 4890 (class 1259 OID 52138)
-- Name: idx_employee_tasks_employee_start; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_employee_tasks_employee_start ON public.employee_tasks USING btree (employee_id, start_time);


--
-- TOC entry 4837 (class 1259 OID 52165)
-- Name: idx_ingredient_batches_supplier_ingredient; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_ingredient_batches_supplier_ingredient ON public.ingredient_batches USING btree (supplier_ingredient_id);


--
-- TOC entry 4891 (class 1259 OID 52159)
-- Name: idx_supplier_ingredient_ingredient; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_supplier_ingredient_ingredient ON public.supplier_ingredients USING btree (ingredient_id);


--
-- TOC entry 4892 (class 1259 OID 52158)
-- Name: idx_supplier_ingredient_supplier; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_supplier_ingredient_supplier ON public.supplier_ingredients USING btree (supplier_id);


--
-- TOC entry 4914 (class 2620 OID 43911)
-- Name: product_batches trg_update_batch_production_minutes; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_update_batch_production_minutes BEFORE INSERT OR UPDATE OF quantity_units, product_id ON public.product_batches FOR EACH ROW EXECUTE FUNCTION public.update_batch_production_minutes();


--
-- TOC entry 4909 (class 2606 OID 35521)
-- Name: companies companies_account_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_account_id_fkey FOREIGN KEY (account_id) REFERENCES public.accounts(account_id) ON DELETE CASCADE;


--
-- TOC entry 4905 (class 2606 OID 35570)
-- Name: cart_items fk_cart_items_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT fk_cart_items_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4906 (class 2606 OID 17475)
-- Name: cart_items fk_cart_items_product_batch_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT fk_cart_items_product_batch_id FOREIGN KEY (batch_id) REFERENCES public.product_batches(product_batch_id);


--
-- TOC entry 4910 (class 2606 OID 43945)
-- Name: employee_tasks fk_employee_tasks_employee_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT fk_employee_tasks_employee_id FOREIGN KEY (employee_id) REFERENCES public.employees(employee_id);


--
-- TOC entry 4911 (class 2606 OID 43950)
-- Name: employee_tasks fk_employee_tasks_order_item_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT fk_employee_tasks_order_item_id FOREIGN KEY (order_item_id) REFERENCES public.order_items(order_item_id);


--
-- TOC entry 4901 (class 2606 OID 35575)
-- Name: favorites fk_favourites_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT fk_favourites_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4902 (class 2606 OID 17104)
-- Name: favorites fk_favourites_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT fk_favourites_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4895 (class 2606 OID 52160)
-- Name: ingredient_batches fk_ingredient_batches_supplier_ingredient; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches
    ADD CONSTRAINT fk_ingredient_batches_supplier_ingredient FOREIGN KEY (supplier_ingredient_id) REFERENCES public.supplier_ingredients(supplier_ingredient_id);


--
-- TOC entry 4903 (class 2606 OID 17406)
-- Name: order_items fk_order_items_order_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT fk_order_items_order_id FOREIGN KEY (order_id) REFERENCES public.orders(order_id);


--
-- TOC entry 4904 (class 2606 OID 17480)
-- Name: order_items fk_order_items_product_batch_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT fk_order_items_product_batch_id FOREIGN KEY (batch_id) REFERENCES public.product_batches(product_batch_id);


--
-- TOC entry 4900 (class 2606 OID 35565)
-- Name: orders fk_orders_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT fk_orders_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4907 (class 2606 OID 17469)
-- Name: product_batches fk_product_batches_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches
    ADD CONSTRAINT fk_product_batches_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4897 (class 2606 OID 16983)
-- Name: product_images fk_product_images_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images
    ADD CONSTRAINT fk_product_images_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4896 (class 2606 OID 16967)
-- Name: products fk_products_category_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT fk_products_category_id FOREIGN KEY (category_id) REFERENCES public.product_categories(product_category_id);


--
-- TOC entry 4898 (class 2606 OID 17001)
-- Name: recipes fk_recipe_ingredient_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_recipe_ingredient_id FOREIGN KEY (ingredient_id) REFERENCES public.ingredients(ingredient_id);


--
-- TOC entry 4899 (class 2606 OID 16996)
-- Name: recipes fk_recipe_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_recipe_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4912 (class 2606 OID 52153)
-- Name: supplier_ingredients fk_supplier_ingredient_ingredient; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supplier_ingredients
    ADD CONSTRAINT fk_supplier_ingredient_ingredient FOREIGN KEY (ingredient_id) REFERENCES public.ingredients(ingredient_id);


--
-- TOC entry 4913 (class 2606 OID 52148)
-- Name: supplier_ingredients fk_supplier_ingredient_supplier; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supplier_ingredients
    ADD CONSTRAINT fk_supplier_ingredient_supplier FOREIGN KEY (supplier_id) REFERENCES public.suppliers(supplier_id);


--
-- TOC entry 4908 (class 2606 OID 35501)
-- Name: users system_users_account_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT system_users_account_id_fkey FOREIGN KEY (account_id) REFERENCES public.accounts(account_id) ON DELETE CASCADE;


-- Completed on 2026-04-10 02:17:11

--
-- PostgreSQL database dump complete
--

\unrestrict FAQjKeWNu4R0hT94S2N1ltoOiCGeWTcJi4vtbdyA5f1YhUw2iuv5pLaSZjXqwki

