export type Route = {
    readonly path: string;
    readonly icon?: string;
    readonly exact?: boolean;
    readonly displayName: string;
    readonly showInNav?: boolean;
    readonly pathAbsolute?: string;
};

export const RoutesConfig = Object.freeze<Record<string, Route>>({
    Home: {
        path: '/',
        showInNav: false,
        icon: 'sign-out-alt',
        displayName: 'Home',
    },
    Login: {
        path: '/login',
        showInNav: true,
        icon: 'sign-out-alt',
        displayName: 'Login',
    },
    Register: {
        path: '/register',
        showInNav: true,
        icon: 'sign-out-alt',
        displayName: 'Register',
    }
});