import { AxiosResponse } from 'axios';
import { BaseService } from './base.service';
import { IAuthUser, ICredentials, ILoginResult } from '../store/auth/types';

/**
 * Auth API abstraction layer communication via Axios (typescript singleton pattern)
 */
class AuthService extends BaseService {
    private static _authService: AuthService;

    private constructor() {
        super();
    }

    public static get Instance(): AuthService {
        return this._authService || (this._authService = new this());
    }

    public async logoutAsync(): Promise<AxiosResponse> {
        return await this.$http.post('logout');
    }

    public async loginAsync(credentials: ICredentials, returnUrl: string): Promise<ILoginResult> {
        const { data } = await this.$http.post<ILoginResult>(`login?ReturnUrl=${returnUrl}`, credentials);
        return data;
    }
}

export const AuthApi = AuthService.Instance;