import { client } from "@/shared/api";
import type { AuthResponse } from "@/shared/api";

export async function me(): Promise <AuthResponse> {
  const { data } = await client.get('api/auth/me');
  return data;
}