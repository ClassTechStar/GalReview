import { ApiClientError } from './api'

const DEFAULT_PURCHASE_URL = 'https://pay.ldxp.cn/shop/7CX09W5E'

export function resolvePurchaseUrl(details?: Record<string, unknown>): string {
  const fromApi = details?.purchaseUrl
  if (typeof fromApi === 'string' && fromApi.startsWith('https://')) return fromApi
  const fromEnv = import.meta.env.VITE_CREDIT_SHOP_URL as string | undefined
  if (fromEnv && fromEnv.trim()) return fromEnv.trim()
  return DEFAULT_PURCHASE_URL
}

export function handleCreditsRequired(reason: unknown): boolean {
  if (!(reason instanceof ApiClientError) || reason.code !== 'CREDITS_INSUFFICIENT') return false
  const balance = typeof reason.details.balance === 'number' ? reason.details.balance : null
  const required = typeof reason.details.required === 'number' ? reason.details.required : null
  const detail = balance !== null && required !== null
    ? `当前可用 ${balance.toFixed(5)} credits，本次至少需要 ${required.toFixed(5)} credits。`
    : '当前 credits 不足。'
  if (window.confirm(`${detail}\n需要先兑换 credits。是否前往购买页面？`)) {
    window.location.assign(resolvePurchaseUrl(reason.details))
  }
  return true
}
