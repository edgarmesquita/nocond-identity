import React, { ReactNode } from 'react';

const renderToastifyMsg = (message: string): ReactNode => (
    <div className="">
        <span>{message}</span>
    </div>
);

export {
    renderToastifyMsg
};