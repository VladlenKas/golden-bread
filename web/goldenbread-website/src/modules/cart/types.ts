export interface CartResponse {

}

export interface UpdateCartItemRequest {
  productId: number,
  productBatchId: number,
  quantity: number
}