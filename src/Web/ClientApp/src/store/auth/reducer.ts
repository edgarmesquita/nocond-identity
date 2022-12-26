import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { IAuthState, ILoginResult, SignInStatus } from './types';

const stateJson = localStorage.getItem('auth');
const initialState: IAuthState = stateJson ? JSON.parse(stateJson) : {
    modelState: null,
    status: SignInStatus.None,
    redirectPath: '',
    isAuthenticated: false
}

const storeState = (state: IAuthState) => {
    localStorage.setItem('auth', JSON.stringify(state));
}

export const slice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        setAuthStatus: (state: IAuthState, action: PayloadAction<SignInStatus>) => {
            state.status = action.payload;
            storeState(state);
        },
        loginSuccess: (state: IAuthState, action: PayloadAction<ILoginResult>) => {
            state.isAuthenticated = action.payload.status == SignInStatus.Succeeded;
            state.status = SignInStatus.Succeeded;
            state.modelState = action.payload.modelState;
            state.redirectPath = action.payload.redirectPath;
            storeState(state);
        },
        loginFail: (state: IAuthState, action: PayloadAction<ILoginResult>) => {
            state.isAuthenticated = action.payload.status != SignInStatus.Succeeded;
            state.status = SignInStatus.Invalid;
            state.modelState = action.payload.modelState;
            storeState(state);
        },
        resetState: (state: IAuthState) => {
            state.modelState = null;
            state.status = SignInStatus.None;
            state.redirectPath = '';
            state.isAuthenticated = false;
            localStorage.removeItem('auth');
        },
    }
});

export default slice.reducer;