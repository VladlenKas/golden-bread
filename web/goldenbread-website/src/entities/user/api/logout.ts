import { client } from "@/shared/api";

export async function logout() {
  return await client.post('/api/auth/logout');
}