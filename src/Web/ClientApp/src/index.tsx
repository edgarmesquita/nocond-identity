import React from 'react';
import ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { Provider } from 'react-redux';

import App from './App';
import { createBrowserHistory } from 'history';
import { ToastContainer } from 'react-toastify';
import store from './store';
import AxiosGlobalConfig from './config/axios.config';

import * as serviceWorker from './serviceWorker';

// Execute global Axios configurations (e.g. request interceptors)
AxiosGlobalConfig.setup();

// This function starts up the React app when it runs in a browser. It sets up the routing configuration and injects the app into a DOM element.
const renderApp = () => {
    ReactDOM.render(
        <AppContainer>
            <Provider store={store}>
                <App />
                <ToastContainer
                    autoClose={3500}
                    draggable={false}
                    newestOnTop={true}
                    position='top-center'
                />
            </Provider>
        </AppContainer>,
        document.getElementById('root')
    );
};

// Execute function above to patch app to DOM
renderApp();

// Allow Hot Module Replacement
if (module.hot) {
    module.hot.accept('./App', () => {
        renderApp();
    });
}

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();