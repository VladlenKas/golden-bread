--
-- PostgreSQL database dump
--

\restrict wquRAMcphkCSaCQplMpYLAV9ugsKyjHyMee446Euev2mZ2MagxtaPSaYvzfG4AA

-- Dumped from database version 17.6
-- Dumped by pg_dump version 17.6

-- Started on 2026-03-27 04:32:29

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
-- TOC entry 930 (class 1247 OID 17144)
-- Name: account_type; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.account_type AS ENUM (
    'user',
    'company'
);


ALTER TYPE public.account_type OWNER TO postgres;

--
-- TOC entry 888 (class 1247 OID 16858)
-- Name: ingredient_batch_status; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.ingredient_batch_status AS ENUM (
    'available',
    'expired',
    'out_of_stock'
);


ALTER TYPE public.ingredient_batch_status OWNER TO postgres;

--
-- TOC entry 897 (class 1247 OID 16882)
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
-- TOC entry 891 (class 1247 OID 16866)
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
-- TOC entry 894 (class 1247 OID 16876)
-- Name: user_role; Type: TYPE; Schema: public; Owner: postgres
--

CREATE TYPE public.user_role AS ENUM (
    'manager_production',
    'admin'
);


ALTER TYPE public.user_role OWNER TO postgres;

--
-- TOC entry 942 (class 1247 OID 17486)
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
-- TOC entry 259 (class 1255 OID 43910)
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
-- TOC entry 246 (class 1259 OID 35479)
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
-- TOC entry 245 (class 1259 OID 35478)
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
-- TOC entry 5165 (class 0 OID 0)
-- Dependencies: 245
-- Name: accounts_account_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.accounts_account_id_seq OWNED BY public.accounts.account_id;


--
-- TOC entry 242 (class 1259 OID 17419)
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
-- TOC entry 241 (class 1259 OID 17418)
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
-- TOC entry 5166 (class 0 OID 0)
-- Dependencies: 241
-- Name: cart_items_new_cart_item_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cart_items_new_cart_item_id_seq OWNED BY public.cart_items.cart_item_id;


--
-- TOC entry 250 (class 1259 OID 35507)
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
-- TOC entry 249 (class 1259 OID 35506)
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
-- TOC entry 5167 (class 0 OID 0)
-- Dependencies: 249
-- Name: companies_company_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.companies_company_id_seq OWNED BY public.companies.company_id;


--
-- TOC entry 257 (class 1259 OID 43935)
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
-- TOC entry 258 (class 1259 OID 43955)
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
-- TOC entry 5168 (class 0 OID 0)
-- Dependencies: 258
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
-- TOC entry 5169 (class 0 OID 0)
-- Dependencies: 217
-- Name: employees_employee_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employees_employee_id_seq OWNED BY public.employees.employee_id;


--
-- TOC entry 238 (class 1259 OID 17092)
-- Name: favorites; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.favorites (
    favorite_id integer NOT NULL,
    product_id integer NOT NULL,
    company_id integer
);


ALTER TABLE public.favorites OWNER TO postgres;

--
-- TOC entry 237 (class 1259 OID 17091)
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
-- TOC entry 5170 (class 0 OID 0)
-- Dependencies: 237
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
    ingredient_id integer NOT NULL,
    purchased_quantity integer NOT NULL,
    remaining_quantity numeric(10,3) NOT NULL,
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
-- TOC entry 5171 (class 0 OID 0)
-- Dependencies: 225
-- Name: ingredient_batches_ingredient_batch_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredient_batches_ingredient_batch_id_seq OWNED BY public.ingredient_batches.ingredient_batch_id;


--
-- TOC entry 252 (class 1259 OID 43781)
-- Name: ingredient_reservations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ingredient_reservations (
    ingredient_reservation_id integer NOT NULL,
    order_id integer NOT NULL,
    ingredient_batch_id integer NOT NULL,
    reserved_quantity numeric(18,2) NOT NULL,
    reserved_at timestamp with time zone DEFAULT now() NOT NULL,
    is_active boolean DEFAULT true NOT NULL,
    is_confirmed boolean DEFAULT false NOT NULL
);


ALTER TABLE public.ingredient_reservations OWNER TO postgres;

--
-- TOC entry 251 (class 1259 OID 43780)
-- Name: ingredient_reservations_ingredient_reservation_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.ingredient_reservations_ingredient_reservation_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.ingredient_reservations_ingredient_reservation_id_seq OWNER TO postgres;

--
-- TOC entry 5172 (class 0 OID 0)
-- Dependencies: 251
-- Name: ingredient_reservations_ingredient_reservation_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredient_reservations_ingredient_reservation_id_seq OWNED BY public.ingredient_reservations.ingredient_reservation_id;


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
    weight numeric(10,3) NOT NULL,
    shelf_life_months integer NOT NULL,
    deleted_at timestamp with time zone
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
-- TOC entry 5173 (class 0 OID 0)
-- Dependencies: 223
-- Name: ingredients_ingredient_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredients_ingredient_id_seq OWNED BY public.ingredients.ingredient_id;


--
-- TOC entry 240 (class 1259 OID 17399)
-- Name: order_items; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.order_items (
    order_item_id integer NOT NULL,
    order_id integer NOT NULL,
    batch_id integer NOT NULL,
    status public.order_status NOT NULL,
    batch_count integer NOT NULL,
    unit_price_at_order numeric(10,2) DEFAULT 0 NOT NULL
);


ALTER TABLE public.order_items OWNER TO postgres;

--
-- TOC entry 239 (class 1259 OID 17398)
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
-- TOC entry 5174 (class 0 OID 0)
-- Dependencies: 239
-- Name: order_items_new_order_item_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.order_items_new_order_item_id_seq OWNED BY public.order_items.order_item_id;


--
-- TOC entry 256 (class 1259 OID 43877)
-- Name: order_production_plan; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.order_production_plan (
    plan_id integer NOT NULL,
    order_id integer NOT NULL,
    order_item_id integer NOT NULL,
    planned_shift_id integer NOT NULL,
    planned_minutes integer NOT NULL,
    employee_id integer NOT NULL,
    assigned_batches integer DEFAULT 1 NOT NULL,
    completed_batches integer DEFAULT 0 NOT NULL,
    CONSTRAINT chk_completed_not_exceed_assigned CHECK ((completed_batches <= assigned_batches)),
    CONSTRAINT order_production_plan_assigned_batches_check CHECK ((assigned_batches > 0)),
    CONSTRAINT order_production_plan_completed_batches_check CHECK ((completed_batches >= 0)),
    CONSTRAINT order_production_plan_planned_minutes_check CHECK ((planned_minutes > 0))
);


ALTER TABLE public.order_production_plan OWNER TO postgres;

--
-- TOC entry 255 (class 1259 OID 43876)
-- Name: order_production_plan_plan_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.order_production_plan_plan_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.order_production_plan_plan_id_seq OWNER TO postgres;

--
-- TOC entry 5175 (class 0 OID 0)
-- Dependencies: 255
-- Name: order_production_plan_plan_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.order_production_plan_plan_id_seq OWNED BY public.order_production_plan.plan_id;


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
    deleted_at timestamp with time zone
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
-- TOC entry 5176 (class 0 OID 0)
-- Dependencies: 233
-- Name: order_tariffs_order_tariff_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.order_tariffs_order_tariff_id_seq OWNED BY public.order_tariffs.order_tariff_id;


--
-- TOC entry 236 (class 1259 OID 17028)
-- Name: orders; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.orders (
    order_id integer NOT NULL,
    tariff_id integer NOT NULL,
    status public.order_status NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    company_id integer NOT NULL,
    "canceled_at " timestamp with time zone,
    cancel_reason text
);


ALTER TABLE public.orders OWNER TO postgres;

--
-- TOC entry 235 (class 1259 OID 17027)
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
-- TOC entry 5177 (class 0 OID 0)
-- Dependencies: 235
-- Name: orders_order_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.orders_order_id_seq OWNED BY public.orders.order_id;


--
-- TOC entry 244 (class 1259 OID 17462)
-- Name: product_batches; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product_batches (
    product_batch_id integer NOT NULL,
    product_id integer NOT NULL,
    quantity_per_batch integer NOT NULL,
    markup_percent integer DEFAULT 0 NOT NULL,
    production_minutes_per_batch integer,
    CONSTRAINT product_batches_new_quantity_check CHECK ((quantity_per_batch > 0))
);


ALTER TABLE public.product_batches OWNER TO postgres;

--
-- TOC entry 243 (class 1259 OID 17461)
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
-- TOC entry 5178 (class 0 OID 0)
-- Dependencies: 243
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
    icon bytea,
    image bytea,
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
-- TOC entry 5179 (class 0 OID 0)
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
-- TOC entry 5180 (class 0 OID 0)
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
-- TOC entry 5181 (class 0 OID 0)
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
-- TOC entry 5182 (class 0 OID 0)
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
-- TOC entry 5183 (class 0 OID 0)
-- Dependencies: 221
-- Name: suppliers_supplier_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.suppliers_supplier_id_seq OWNED BY public.suppliers.supplier_id;


--
-- TOC entry 248 (class 1259 OID 35493)
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
-- TOC entry 247 (class 1259 OID 35492)
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
-- TOC entry 5184 (class 0 OID 0)
-- Dependencies: 247
-- Name: system_users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.system_users_user_id_seq OWNED BY public.users.user_id;


--
-- TOC entry 254 (class 1259 OID 43848)
-- Name: work_shifts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.work_shifts (
    shift_id integer NOT NULL,
    employee_id integer NOT NULL,
    shift_date date NOT NULL,
    start_time time without time zone DEFAULT '08:00:00'::time without time zone NOT NULL,
    break_start time without time zone DEFAULT '12:00:00'::time without time zone NOT NULL,
    break_end time without time zone DEFAULT '13:00:00'::time without time zone NOT NULL,
    end_time time without time zone DEFAULT '17:00:00'::time without time zone NOT NULL,
    is_working boolean DEFAULT true,
    created_at timestamp with time zone DEFAULT now()
);


ALTER TABLE public.work_shifts OWNER TO postgres;

--
-- TOC entry 253 (class 1259 OID 43847)
-- Name: work_shifts_shift_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.work_shifts_shift_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.work_shifts_shift_id_seq OWNER TO postgres;

--
-- TOC entry 5185 (class 0 OID 0)
-- Dependencies: 253
-- Name: work_shifts_shift_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.work_shifts_shift_id_seq OWNED BY public.work_shifts.shift_id;


--
-- TOC entry 4834 (class 2604 OID 35482)
-- Name: accounts account_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts ALTER COLUMN account_id SET DEFAULT nextval('public.accounts_account_id_seq'::regclass);


--
-- TOC entry 4831 (class 2604 OID 17422)
-- Name: cart_items cart_item_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items ALTER COLUMN cart_item_id SET DEFAULT nextval('public.cart_items_new_cart_item_id_seq'::regclass);


--
-- TOC entry 4838 (class 2604 OID 35510)
-- Name: companies company_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies ALTER COLUMN company_id SET DEFAULT nextval('public.companies_company_id_seq'::regclass);


--
-- TOC entry 4853 (class 2604 OID 43956)
-- Name: employee_tasks employee_task_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks ALTER COLUMN employee_task_id SET DEFAULT nextval('public.employee_tasks_new_employee_task_id_seq'::regclass);


--
-- TOC entry 4814 (class 2604 OID 16907)
-- Name: employees employee_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees ALTER COLUMN employee_id SET DEFAULT nextval('public.employees_employee_id_seq'::regclass);


--
-- TOC entry 4828 (class 2604 OID 17095)
-- Name: favorites favorite_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites ALTER COLUMN favorite_id SET DEFAULT nextval('public.favourites_favourite_id_seq'::regclass);


--
-- TOC entry 4818 (class 2604 OID 16948)
-- Name: ingredient_batches ingredient_batch_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches ALTER COLUMN ingredient_batch_id SET DEFAULT nextval('public.ingredient_batches_ingredient_batch_id_seq'::regclass);


--
-- TOC entry 4839 (class 2604 OID 43784)
-- Name: ingredient_reservations ingredient_reservation_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_reservations ALTER COLUMN ingredient_reservation_id SET DEFAULT nextval('public.ingredient_reservations_ingredient_reservation_id_seq'::regclass);


--
-- TOC entry 4817 (class 2604 OID 16932)
-- Name: ingredients ingredient_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients ALTER COLUMN ingredient_id SET DEFAULT nextval('public.ingredients_ingredient_id_seq'::regclass);


--
-- TOC entry 4829 (class 2604 OID 17402)
-- Name: order_items order_item_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items ALTER COLUMN order_item_id SET DEFAULT nextval('public.order_items_new_order_item_id_seq'::regclass);


--
-- TOC entry 4850 (class 2604 OID 43880)
-- Name: order_production_plan plan_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_production_plan ALTER COLUMN plan_id SET DEFAULT nextval('public.order_production_plan_plan_id_seq'::regclass);


--
-- TOC entry 4825 (class 2604 OID 17012)
-- Name: order_tariffs order_tariff_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_tariffs ALTER COLUMN order_tariff_id SET DEFAULT nextval('public.order_tariffs_order_tariff_id_seq'::regclass);


--
-- TOC entry 4826 (class 2604 OID 17031)
-- Name: orders order_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders ALTER COLUMN order_id SET DEFAULT nextval('public.orders_order_id_seq'::regclass);


--
-- TOC entry 4832 (class 2604 OID 17465)
-- Name: product_batches product_batch_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches ALTER COLUMN product_batch_id SET DEFAULT nextval('public.product_batches_new_product_batch_id_seq'::regclass);


--
-- TOC entry 4815 (class 2604 OID 16915)
-- Name: product_categories product_category_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_categories ALTER COLUMN product_category_id SET DEFAULT nextval('public.production_categories_production_category_id_seq'::regclass);


--
-- TOC entry 4823 (class 2604 OID 16977)
-- Name: product_images product_image_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images ALTER COLUMN product_image_id SET DEFAULT nextval('public.product_images_product_image_id_seq'::regclass);


--
-- TOC entry 4819 (class 2604 OID 16961)
-- Name: products product_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products ALTER COLUMN product_id SET DEFAULT nextval('public.products_product_id_seq'::regclass);


--
-- TOC entry 4824 (class 2604 OID 16993)
-- Name: recipes recipe_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes ALTER COLUMN recipe_id SET DEFAULT nextval('public.recipes_recipe_id_seq'::regclass);


--
-- TOC entry 4816 (class 2604 OID 16922)
-- Name: suppliers supplier_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suppliers ALTER COLUMN supplier_id SET DEFAULT nextval('public.suppliers_supplier_id_seq'::regclass);


--
-- TOC entry 4837 (class 2604 OID 35496)
-- Name: users user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.system_users_user_id_seq'::regclass);


--
-- TOC entry 4843 (class 2604 OID 43851)
-- Name: work_shifts shift_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.work_shifts ALTER COLUMN shift_id SET DEFAULT nextval('public.work_shifts_shift_id_seq'::regclass);


--
-- TOC entry 5147 (class 0 OID 35479)
-- Dependencies: 246
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
-- TOC entry 5143 (class 0 OID 17419)
-- Dependencies: 242
-- Data for Name: cart_items; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.cart_items (cart_item_id, batch_id, quantity, company_id) FROM stdin;
85	7	1	23
82	9	4	23
86	6	1	23
84	13	3	23
\.


--
-- TOC entry 5151 (class 0 OID 35507)
-- Dependencies: 250
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
-- TOC entry 5158 (class 0 OID 43935)
-- Dependencies: 257
-- Data for Name: employee_tasks; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.employee_tasks (employee_task_id, employee_id, order_item_id, status, assigned_quantity, completed_quantity, start_time, end_time) FROM stdin;
\.


--
-- TOC entry 5119 (class 0 OID 16904)
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
-- TOC entry 5139 (class 0 OID 17092)
-- Dependencies: 238
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
-- TOC entry 5127 (class 0 OID 16945)
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
-- TOC entry 5153 (class 0 OID 43781)
-- Dependencies: 252
-- Data for Name: ingredient_reservations; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredient_reservations (ingredient_reservation_id, order_id, ingredient_batch_id, reserved_quantity, reserved_at, is_active, is_confirmed) FROM stdin;
\.


--
-- TOC entry 5125 (class 0 OID 16929)
-- Dependencies: 224
-- Data for Name: ingredients; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredients (ingredient_id, supplier_id, name, price, unit, weight, shelf_life_months, deleted_at) FROM stdin;
1	1	Мука пшеничная	45.00	kg	1.000	12	\N
2	1	Молоко	65.00	l	1.000	1	\N
3	1	Сахар	50.00	kg	1.000	24	\N
4	1	Соль	15.00	kg	1.000	36	\N
5	1	Дрожжи свежие	120.00	kg	0.500	3	\N
6	1	Яйца	90.00	pcs	0.060	1	\N
7	1	Масло сливочное	550.00	kg	1.000	6	\N
8	1	Корица молотая	350.00	kg	0.100	18	\N
9	1	Изюм	280.00	kg	1.000	12	\N
10	1	Ванилин	450.00	kg	0.050	24	\N
11	1	Мак молотый	420.00	kg	1.000	18	\N
12	1	Яблоки	120.00	kg	1.000	2	\N
13	1	Вишня	180.00	kg	1.000	6	\N
14	1	Говядина фарш	450.00	kg	1.000	3	\N
15	1	Лук репчатый	40.00	kg	1.000	3	\N
16	1	Мука ржаная	55.00	kg	1.000	12	\N
17	1	Кориандр	280.00	kg	0.100	24	\N
18	1	Мед	350.00	kg	1.000	36	\N
19	1	Сметана	180.00	kg	1.000	2	\N
20	1	Сыр сливочный	650.00	kg	1.000	2	\N
21	1	Сахарная пудра	80.00	kg	1.000	24	\N
\.


--
-- TOC entry 5141 (class 0 OID 17399)
-- Dependencies: 240
-- Data for Name: order_items; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.order_items (order_item_id, order_id, batch_id, status, batch_count, unit_price_at_order) FROM stdin;
\.


--
-- TOC entry 5157 (class 0 OID 43877)
-- Dependencies: 256
-- Data for Name: order_production_plan; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.order_production_plan (plan_id, order_id, order_item_id, planned_shift_id, planned_minutes, employee_id, assigned_batches, completed_batches) FROM stdin;
\.


--
-- TOC entry 5135 (class 0 OID 17009)
-- Dependencies: 234
-- Data for Name: order_tariffs; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.order_tariffs (order_tariff_id, name, description, markup_percent, free_employees_percent, deleted_at) FROM stdin;
1	Стандартный	Без наценки, 10% сотрудников	0.00	20.00	\N
2	Эконом	Наценка 15%, 20% сотрудников	15.00	20.00	\N
3	Экспресс	Наценка 25%, 30% сотрудников	25.00	30.00	\N
\.


--
-- TOC entry 5137 (class 0 OID 17028)
-- Dependencies: 236
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orders (order_id, tariff_id, status, start_date, end_date, created_at, company_id, "canceled_at ", cancel_reason) FROM stdin;
1	1	completed	2025-12-01	2025-12-01	2025-12-01 10:00:00+05	18	\N	\N
2	1	completed	2025-12-05	2025-12-05	2025-12-05 11:30:00+05	18	\N	\N
3	1	completed	2025-12-10	2025-12-10	2025-12-10 14:20:00+05	18	\N	\N
4	1	in_progress	2025-12-15	2025-12-15	2025-12-15 09:15:00+05	18	\N	\N
5	1	completed	2025-11-05	2025-11-05	2025-11-05 12:00:00+05	18	\N	\N
6	1	completed	2025-11-12	2025-11-12	2025-11-12 15:30:00+05	18	\N	\N
\.


--
-- TOC entry 5145 (class 0 OID 17462)
-- Dependencies: 244
-- Data for Name: product_batches; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_batches (product_batch_id, product_id, quantity_per_batch, markup_percent, production_minutes_per_batch) FROM stdin;
1	1	10	115	450
2	2	12	110	600
3	3	10	120	550
4	4	5	90	600
5	5	4	85	520
6	6	3	95	450
7	7	8	70	1440
8	8	10	60	900
9	9	6	75	1200
10	10	2	60	480
11	11	2	55	420
12	12	3	65	540
13	5	10	80	1300
\.


--
-- TOC entry 5121 (class 0 OID 16912)
-- Dependencies: 220
-- Data for Name: product_categories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product_categories (product_category_id, name, color, icon, image, deleted_at) FROM stdin;
1	Булочки	FFD700	\N	\N	\N
2	Пироги	FF6347	\N	\N	\N
3	Хлеб	D2691E	\N	\N	\N
4	Торты	FF69B4	\N	\N	\N
\.


--
-- TOC entry 5131 (class 0 OID 16974)
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
-- TOC entry 5129 (class 0 OID 16958)
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
-- TOC entry 5133 (class 0 OID 16990)
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
-- TOC entry 5123 (class 0 OID 16919)
-- Dependencies: 222
-- Data for Name: suppliers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.suppliers (supplier_id, name, email, phone, address, deleted_at) FROM stdin;
1	ООО "ПродИнгредиент"	info@prodingredient.ru	79171234567	г. Уфа, ул. Складская, 15	\N
\.


--
-- TOC entry 5149 (class 0 OID 35493)
-- Dependencies: 248
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (user_id, account_id, role, firstname, lastname, patronymic, birthday) FROM stdin;
1	16	admin	asdf	asdf		2020-01-01
2	17	manager_production	фываф	фаф	Антонович	2006-01-01
3	18	admin	Владлен	Касимов	Ильшатович	2006-01-08
\.


--
-- TOC entry 5155 (class 0 OID 43848)
-- Dependencies: 254
-- Data for Name: work_shifts; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.work_shifts (shift_id, employee_id, shift_date, start_time, break_start, break_end, end_time, is_working, created_at) FROM stdin;
\.


--
-- TOC entry 5186 (class 0 OID 0)
-- Dependencies: 245
-- Name: accounts_account_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.accounts_account_id_seq', 47, true);


--
-- TOC entry 5187 (class 0 OID 0)
-- Dependencies: 241
-- Name: cart_items_new_cart_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.cart_items_new_cart_item_id_seq', 86, true);


--
-- TOC entry 5188 (class 0 OID 0)
-- Dependencies: 249
-- Name: companies_company_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.companies_company_id_seq', 28, true);


--
-- TOC entry 5189 (class 0 OID 0)
-- Dependencies: 258
-- Name: employee_tasks_new_employee_task_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employee_tasks_new_employee_task_id_seq', 1, false);


--
-- TOC entry 5190 (class 0 OID 0)
-- Dependencies: 217
-- Name: employees_employee_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employees_employee_id_seq', 20, true);


--
-- TOC entry 5191 (class 0 OID 0)
-- Dependencies: 237
-- Name: favourites_favourite_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.favourites_favourite_id_seq', 74, true);


--
-- TOC entry 5192 (class 0 OID 0)
-- Dependencies: 225
-- Name: ingredient_batches_ingredient_batch_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredient_batches_ingredient_batch_id_seq', 9, true);


--
-- TOC entry 5193 (class 0 OID 0)
-- Dependencies: 251
-- Name: ingredient_reservations_ingredient_reservation_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredient_reservations_ingredient_reservation_id_seq', 1, false);


--
-- TOC entry 5194 (class 0 OID 0)
-- Dependencies: 223
-- Name: ingredients_ingredient_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredients_ingredient_id_seq', 21, true);


--
-- TOC entry 5195 (class 0 OID 0)
-- Dependencies: 239
-- Name: order_items_new_order_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.order_items_new_order_item_id_seq', 12, true);


--
-- TOC entry 5196 (class 0 OID 0)
-- Dependencies: 255
-- Name: order_production_plan_plan_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.order_production_plan_plan_id_seq', 1, false);


--
-- TOC entry 5197 (class 0 OID 0)
-- Dependencies: 233
-- Name: order_tariffs_order_tariff_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.order_tariffs_order_tariff_id_seq', 4, true);


--
-- TOC entry 5198 (class 0 OID 0)
-- Dependencies: 235
-- Name: orders_order_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orders_order_id_seq', 6, true);


--
-- TOC entry 5199 (class 0 OID 0)
-- Dependencies: 243
-- Name: product_batches_new_product_batch_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.product_batches_new_product_batch_id_seq', 13, true);


--
-- TOC entry 5200 (class 0 OID 0)
-- Dependencies: 229
-- Name: product_images_product_image_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.product_images_product_image_id_seq', 13, true);


--
-- TOC entry 5201 (class 0 OID 0)
-- Dependencies: 219
-- Name: production_categories_production_category_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.production_categories_production_category_id_seq', 4, true);


--
-- TOC entry 5202 (class 0 OID 0)
-- Dependencies: 227
-- Name: products_product_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.products_product_id_seq', 12, true);


--
-- TOC entry 5203 (class 0 OID 0)
-- Dependencies: 231
-- Name: recipes_recipe_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.recipes_recipe_id_seq', 180, true);


--
-- TOC entry 5204 (class 0 OID 0)
-- Dependencies: 221
-- Name: suppliers_supplier_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.suppliers_supplier_id_seq', 1, true);


--
-- TOC entry 5205 (class 0 OID 0)
-- Dependencies: 247
-- Name: system_users_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.system_users_user_id_seq', 3, true);


--
-- TOC entry 5206 (class 0 OID 0)
-- Dependencies: 253
-- Name: work_shifts_shift_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.work_shifts_shift_id_seq', 1, false);


--
-- TOC entry 4907 (class 2606 OID 35489)
-- Name: accounts accounts_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.accounts
    ADD CONSTRAINT accounts_pkey PRIMARY KEY (account_id);


--
-- TOC entry 4899 (class 2606 OID 17425)
-- Name: cart_items cart_items_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT cart_items_new_pkey PRIMARY KEY (cart_item_id);


--
-- TOC entry 4913 (class 2606 OID 35554)
-- Name: companies companies_inn_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_inn_key UNIQUE (inn);


--
-- TOC entry 4915 (class 2606 OID 35552)
-- Name: companies companies_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_name_key UNIQUE (name);


--
-- TOC entry 4917 (class 2606 OID 35518)
-- Name: companies companies_ogrn_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_ogrn_key UNIQUE (ogrn);


--
-- TOC entry 4919 (class 2606 OID 35514)
-- Name: companies companies_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_pkey PRIMARY KEY (company_id);


--
-- TOC entry 4941 (class 2606 OID 43942)
-- Name: employee_tasks employee_tasks_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT employee_tasks_new_pkey PRIMARY KEY (employee_task_id);


--
-- TOC entry 4863 (class 2606 OID 16910)
-- Name: employees employees_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (employee_id);


--
-- TOC entry 4891 (class 2606 OID 17098)
-- Name: favorites favourites_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT favourites_pkey PRIMARY KEY (favorite_id);


--
-- TOC entry 4873 (class 2606 OID 16950)
-- Name: ingredient_batches ingredient_batches_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches
    ADD CONSTRAINT ingredient_batches_pkey PRIMARY KEY (ingredient_batch_id);


--
-- TOC entry 4926 (class 2606 OID 43788)
-- Name: ingredient_reservations ingredient_reservations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_reservations
    ADD CONSTRAINT ingredient_reservations_pkey PRIMARY KEY (ingredient_reservation_id);


--
-- TOC entry 4870 (class 2606 OID 16937)
-- Name: ingredients ingredients_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients
    ADD CONSTRAINT ingredients_pkey PRIMARY KEY (ingredient_id);


--
-- TOC entry 4897 (class 2606 OID 17405)
-- Name: order_items order_items_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT order_items_new_pkey PRIMARY KEY (order_item_id);


--
-- TOC entry 4939 (class 2606 OID 43884)
-- Name: order_production_plan order_production_plan_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_production_plan
    ADD CONSTRAINT order_production_plan_pkey PRIMARY KEY (plan_id);


--
-- TOC entry 4885 (class 2606 OID 17016)
-- Name: order_tariffs order_tariffs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_tariffs
    ADD CONSTRAINT order_tariffs_pkey PRIMARY KEY (order_tariff_id);


--
-- TOC entry 4889 (class 2606 OID 17033)
-- Name: orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (order_id);


--
-- TOC entry 4904 (class 2606 OID 17468)
-- Name: product_batches product_batches_new_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches
    ADD CONSTRAINT product_batches_new_pkey PRIMARY KEY (product_batch_id);


--
-- TOC entry 4879 (class 2606 OID 16982)
-- Name: product_images product_images_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images
    ADD CONSTRAINT product_images_pkey PRIMARY KEY (product_image_id);


--
-- TOC entry 4865 (class 2606 OID 16917)
-- Name: product_categories production_categories_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_categories
    ADD CONSTRAINT production_categories_pkey PRIMARY KEY (product_category_id);


--
-- TOC entry 4876 (class 2606 OID 16966)
-- Name: products products_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT products_pkey PRIMARY KEY (product_id);


--
-- TOC entry 4883 (class 2606 OID 16995)
-- Name: recipes recipes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT recipes_pkey PRIMARY KEY (recipe_id);


--
-- TOC entry 4867 (class 2606 OID 16927)
-- Name: suppliers suppliers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suppliers
    ADD CONSTRAINT suppliers_pkey PRIMARY KEY (supplier_id);


--
-- TOC entry 4909 (class 2606 OID 35498)
-- Name: users system_users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT system_users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4921 (class 2606 OID 35520)
-- Name: companies unique_account_per_company; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT unique_account_per_company UNIQUE (account_id);


--
-- TOC entry 4911 (class 2606 OID 35500)
-- Name: users unique_account_per_user; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT unique_account_per_user UNIQUE (account_id);


--
-- TOC entry 4930 (class 2606 OID 43861)
-- Name: work_shifts work_shifts_employee_id_shift_date_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.work_shifts
    ADD CONSTRAINT work_shifts_employee_id_shift_date_key UNIQUE (employee_id, shift_date);


--
-- TOC entry 4932 (class 2606 OID 43859)
-- Name: work_shifts work_shifts_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.work_shifts
    ADD CONSTRAINT work_shifts_pkey PRIMARY KEY (shift_id);


--
-- TOC entry 4905 (class 1259 OID 43743)
-- Name: accounts_email_active_unique; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX accounts_email_active_unique ON public.accounts USING btree (((deleted_at IS NULL))) INCLUDE (deleted_at) WITH (deduplicate_items='true');


--
-- TOC entry 4900 (class 1259 OID 35535)
-- Name: fk_cart_items_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_cart_items_account_id_idx ON public.cart_items USING btree (company_id);


--
-- TOC entry 4901 (class 1259 OID 17437)
-- Name: fk_cart_items_product_batch_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_cart_items_product_batch_id_idx ON public.cart_items USING btree (batch_id);


--
-- TOC entry 4942 (class 1259 OID 43943)
-- Name: fk_employee_tasks_employee_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_employee_tasks_employee_id_idx ON public.employee_tasks USING btree (employee_id);


--
-- TOC entry 4943 (class 1259 OID 43944)
-- Name: fk_employee_tasks_order_item_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_employee_tasks_order_item_id_idx ON public.employee_tasks USING btree (order_item_id);


--
-- TOC entry 4892 (class 1259 OID 35541)
-- Name: fk_favorites_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_favorites_account_id_idx ON public.favorites USING btree (company_id);


--
-- TOC entry 4893 (class 1259 OID 17110)
-- Name: fk_favorites_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_favorites_product_id_idx ON public.favorites USING btree (product_id);


--
-- TOC entry 4871 (class 1259 OID 16956)
-- Name: fk_ingredient_batches_ingredient_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_ingredient_batches_ingredient_id_idx ON public.ingredient_batches USING btree (ingredient_id);


--
-- TOC entry 4868 (class 1259 OID 16943)
-- Name: fk_ingredients_supplier_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_ingredients_supplier_id_idx ON public.ingredients USING btree (supplier_id);


--
-- TOC entry 4894 (class 1259 OID 17416)
-- Name: fk_order_items_order_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_order_items_order_id_idx ON public.order_items USING btree (order_id);


--
-- TOC entry 4895 (class 1259 OID 17417)
-- Name: fk_order_items_product_batch_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_order_items_product_batch_id_idx ON public.order_items USING btree (batch_id);


--
-- TOC entry 4886 (class 1259 OID 35547)
-- Name: fk_orders_account_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_orders_account_id_idx ON public.orders USING btree (company_id);


--
-- TOC entry 4887 (class 1259 OID 17050)
-- Name: fk_orders_tariff_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_orders_tariff_id_idx ON public.orders USING btree (tariff_id);


--
-- TOC entry 4902 (class 1259 OID 17474)
-- Name: fk_product_batches_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_product_batches_product_id_idx ON public.product_batches USING btree (product_id);


--
-- TOC entry 4877 (class 1259 OID 16988)
-- Name: fk_product_images_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_product_images_product_id_idx ON public.product_images USING btree (product_id);


--
-- TOC entry 4874 (class 1259 OID 16972)
-- Name: fk_products_category_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_products_category_id_idx ON public.products USING btree (category_id);


--
-- TOC entry 4880 (class 1259 OID 17007)
-- Name: fk_recipe_ingredient_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_recipe_ingredient_id_idx ON public.recipes USING btree (ingredient_id);


--
-- TOC entry 4881 (class 1259 OID 17006)
-- Name: fk_recipe_product_id_idx; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX fk_recipe_product_id_idx ON public.recipes USING btree (product_id);


--
-- TOC entry 4922 (class 1259 OID 43811)
-- Name: idx_ingredient_reservations_active; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_ingredient_reservations_active ON public.ingredient_reservations USING btree (is_active) WHERE (is_active = true);


--
-- TOC entry 4923 (class 1259 OID 43810)
-- Name: idx_ingredient_reservations_batch_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_ingredient_reservations_batch_id ON public.ingredient_reservations USING btree (ingredient_batch_id);


--
-- TOC entry 4924 (class 1259 OID 43809)
-- Name: idx_ingredient_reservations_order_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_ingredient_reservations_order_id ON public.ingredient_reservations USING btree (order_id);


--
-- TOC entry 4933 (class 1259 OID 43933)
-- Name: idx_order_plan_employee; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_order_plan_employee ON public.order_production_plan USING btree (employee_id) WHERE (completed_batches < assigned_batches);


--
-- TOC entry 4934 (class 1259 OID 43907)
-- Name: idx_order_plan_order_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_order_plan_order_id ON public.order_production_plan USING btree (order_id);


--
-- TOC entry 4935 (class 1259 OID 43909)
-- Name: idx_order_plan_order_item; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_order_plan_order_item ON public.order_production_plan USING btree (order_item_id);


--
-- TOC entry 4936 (class 1259 OID 43934)
-- Name: idx_order_plan_shift; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_order_plan_shift ON public.order_production_plan USING btree (planned_shift_id);


--
-- TOC entry 4937 (class 1259 OID 43908)
-- Name: idx_order_plan_shift_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_order_plan_shift_id ON public.order_production_plan USING btree (planned_shift_id);


--
-- TOC entry 4927 (class 1259 OID 43867)
-- Name: idx_work_shifts_date_employee; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_work_shifts_date_employee ON public.work_shifts USING btree (shift_date, employee_id) WHERE (is_working = true);


--
-- TOC entry 4928 (class 1259 OID 43868)
-- Name: idx_work_shifts_employee_date; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_work_shifts_employee_date ON public.work_shifts USING btree (employee_id, shift_date);


--
-- TOC entry 4972 (class 2620 OID 43911)
-- Name: product_batches trg_update_batch_production_minutes; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_update_batch_production_minutes BEFORE INSERT OR UPDATE OF quantity_per_batch, product_id ON public.product_batches FOR EACH ROW EXECUTE FUNCTION public.update_batch_production_minutes();


--
-- TOC entry 4960 (class 2606 OID 35521)
-- Name: companies companies_account_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT companies_account_id_fkey FOREIGN KEY (account_id) REFERENCES public.accounts(account_id) ON DELETE CASCADE;


--
-- TOC entry 4961 (class 2606 OID 43804)
-- Name: ingredient_reservations fk_batch; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_reservations
    ADD CONSTRAINT fk_batch FOREIGN KEY (ingredient_batch_id) REFERENCES public.ingredient_batches(ingredient_batch_id);


--
-- TOC entry 4956 (class 2606 OID 35570)
-- Name: cart_items fk_cart_items_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT fk_cart_items_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4957 (class 2606 OID 17475)
-- Name: cart_items fk_cart_items_product_batch_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cart_items
    ADD CONSTRAINT fk_cart_items_product_batch_id FOREIGN KEY (batch_id) REFERENCES public.product_batches(product_batch_id);


--
-- TOC entry 4970 (class 2606 OID 43945)
-- Name: employee_tasks fk_employee_tasks_employee_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT fk_employee_tasks_employee_id FOREIGN KEY (employee_id) REFERENCES public.employees(employee_id);


--
-- TOC entry 4971 (class 2606 OID 43950)
-- Name: employee_tasks fk_employee_tasks_order_item_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employee_tasks
    ADD CONSTRAINT fk_employee_tasks_order_item_id FOREIGN KEY (order_item_id) REFERENCES public.order_items(order_item_id);


--
-- TOC entry 4952 (class 2606 OID 35575)
-- Name: favorites fk_favourites_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT fk_favourites_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4953 (class 2606 OID 17104)
-- Name: favorites fk_favourites_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.favorites
    ADD CONSTRAINT fk_favourites_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4945 (class 2606 OID 16951)
-- Name: ingredient_batches fk_ingredient_batches_ingredient_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_batches
    ADD CONSTRAINT fk_ingredient_batches_ingredient_id FOREIGN KEY (ingredient_id) REFERENCES public.ingredients(ingredient_id);


--
-- TOC entry 4944 (class 2606 OID 16938)
-- Name: ingredients fk_ingredients_supplier_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredients
    ADD CONSTRAINT fk_ingredients_supplier_id FOREIGN KEY (supplier_id) REFERENCES public.suppliers(supplier_id);


--
-- TOC entry 4962 (class 2606 OID 43799)
-- Name: ingredient_reservations fk_order; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_reservations
    ADD CONSTRAINT fk_order FOREIGN KEY (order_id) REFERENCES public.orders(order_id);


--
-- TOC entry 4954 (class 2606 OID 17406)
-- Name: order_items fk_order_items_order_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT fk_order_items_order_id FOREIGN KEY (order_id) REFERENCES public.orders(order_id);


--
-- TOC entry 4955 (class 2606 OID 17480)
-- Name: order_items fk_order_items_product_batch_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_items
    ADD CONSTRAINT fk_order_items_product_batch_id FOREIGN KEY (batch_id) REFERENCES public.product_batches(product_batch_id);


--
-- TOC entry 4950 (class 2606 OID 35565)
-- Name: orders fk_orders_company_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT fk_orders_company_id FOREIGN KEY (company_id) REFERENCES public.companies(company_id) NOT VALID;


--
-- TOC entry 4951 (class 2606 OID 17039)
-- Name: orders fk_orders_tariff_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT fk_orders_tariff_id FOREIGN KEY (tariff_id) REFERENCES public.order_tariffs(order_tariff_id);


--
-- TOC entry 4958 (class 2606 OID 17469)
-- Name: product_batches fk_product_batches_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_batches
    ADD CONSTRAINT fk_product_batches_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4947 (class 2606 OID 16983)
-- Name: product_images fk_product_images_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product_images
    ADD CONSTRAINT fk_product_images_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4946 (class 2606 OID 16967)
-- Name: products fk_products_category_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT fk_products_category_id FOREIGN KEY (category_id) REFERENCES public.product_categories(product_category_id);


--
-- TOC entry 4948 (class 2606 OID 17001)
-- Name: recipes fk_recipe_ingredient_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_recipe_ingredient_id FOREIGN KEY (ingredient_id) REFERENCES public.ingredients(ingredient_id);


--
-- TOC entry 4949 (class 2606 OID 16996)
-- Name: recipes fk_recipe_product_id; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.recipes
    ADD CONSTRAINT fk_recipe_product_id FOREIGN KEY (product_id) REFERENCES public.products(product_id);


--
-- TOC entry 4963 (class 2606 OID 43794)
-- Name: ingredient_reservations ingredient_reservations_ingredient_batch_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_reservations
    ADD CONSTRAINT ingredient_reservations_ingredient_batch_id_fkey FOREIGN KEY (ingredient_batch_id) REFERENCES public.ingredient_batches(ingredient_batch_id);


--
-- TOC entry 4964 (class 2606 OID 43789)
-- Name: ingredient_reservations ingredient_reservations_order_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient_reservations
    ADD CONSTRAINT ingredient_reservations_order_id_fkey FOREIGN KEY (order_id) REFERENCES public.orders(order_id) ON DELETE CASCADE;


--
-- TOC entry 4966 (class 2606 OID 43925)
-- Name: order_production_plan order_production_plan_employee_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_production_plan
    ADD CONSTRAINT order_production_plan_employee_id_fkey FOREIGN KEY (employee_id) REFERENCES public.employees(employee_id);


--
-- TOC entry 4967 (class 2606 OID 43887)
-- Name: order_production_plan order_production_plan_order_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_production_plan
    ADD CONSTRAINT order_production_plan_order_id_fkey FOREIGN KEY (order_id) REFERENCES public.orders(order_id) ON DELETE CASCADE;


--
-- TOC entry 4968 (class 2606 OID 43892)
-- Name: order_production_plan order_production_plan_order_item_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_production_plan
    ADD CONSTRAINT order_production_plan_order_item_id_fkey FOREIGN KEY (order_item_id) REFERENCES public.order_items(order_item_id);


--
-- TOC entry 4969 (class 2606 OID 43902)
-- Name: order_production_plan order_production_plan_planned_shift_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.order_production_plan
    ADD CONSTRAINT order_production_plan_planned_shift_id_fkey FOREIGN KEY (planned_shift_id) REFERENCES public.work_shifts(shift_id);


--
-- TOC entry 4959 (class 2606 OID 35501)
-- Name: users system_users_account_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT system_users_account_id_fkey FOREIGN KEY (account_id) REFERENCES public.accounts(account_id) ON DELETE CASCADE;


--
-- TOC entry 4965 (class 2606 OID 43862)
-- Name: work_shifts work_shifts_employee_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.work_shifts
    ADD CONSTRAINT work_shifts_employee_id_fkey FOREIGN KEY (employee_id) REFERENCES public.employees(employee_id);


-- Completed on 2026-03-27 04:32:30

--
-- PostgreSQL database dump complete
--

\unrestrict wquRAMcphkCSaCQplMpYLAV9ugsKyjHyMee446Euev2mZ2MagxtaPSaYvzfG4AA

