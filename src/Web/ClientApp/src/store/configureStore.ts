import { combineReducers, configureStore } from '@reduxjs/toolkit';
import AuthReducer from './auth/reducer';

const reducer = combineReducers({
    auth: AuthReducer
})

export const store = configureStore({ reducer });