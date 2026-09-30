import { afterEach, describe, expect, it, vi } from 'vitest'

vi.mock('./api', () => ({ ApiClientError: class ApiClientError extends Error {} }))

describe('resolvePurchaseUrl', () => {
  afterEach(() => {
    vi.resetModules()
    vi.unstubAllEnvs()
  })

  it('优先使用 API 返回的 https purchaseUrl', async () => {
    const { resolvePurchaseUrl } = await import('./credits')
    expect(resolvePurchaseUrl({ purchaseUrl: 'https://shop.example/a' })).toBe('https://shop.example/a')
  })

  it('拒绝非 https 的 API 地址', async () => {
    const { resolvePurchaseUrl } = await import('./credits')
    const url = resolvePurchaseUrl({ purchaseUrl: 'http://evil.example' })
    expect(url.startsWith('https://')).toBe(true)
  })

  it('回退到默认购买地址', async () => {
    const { resolvePurchaseUrl } = await import('./credits')
    expect(resolvePurchaseUrl({})).toBe('https://pay.ldxp.cn/shop/7CX09W5E')
  })
})
