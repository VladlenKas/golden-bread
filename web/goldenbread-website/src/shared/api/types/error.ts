export enum ErrorKind {
  Network = 'network',
  Http = 'http',
  Validation = 'validation',
  Unknown = 'unknown'
}

export interface ApiError {
  message: string;
  kind: ErrorKind;
  status: number;
  data: any;
}