import React, { ReactNode } from "react";
import clsx from 'clsx';
import {
    withStyles,
    createStyles,
    Theme,
    WithStyles,
    StyleRules
} from "@material-ui/core/styles";

const styles: (theme: Theme) => StyleRules<string> = theme =>
    createStyles({
        toolbar: {
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'flex-end',
            padding: theme.spacing(0, 1),
            // necessary for content to be below app bar
            ...theme.mixins.toolbar,
        },
        content: {
            flexGrow: 1,
            padding: theme.spacing(3),
            maxWidth: 'calc(100% - 57px)',
        },

        contentWithDrawer: {
            maxWidth: 'calc(100% - 240px)'
        }
    });

type AppContentProps = {
    drawerOpen?: boolean;
    children?: ReactNode
} & WithStyles<typeof styles>;

const AppContent = ({ classes, children, drawerOpen }: AppContentProps) => {
    return (
        <main className={clsx(classes.content, {
            [classes.contentWithDrawer]: drawerOpen
        })}>
            <div className={classes.toolbar} />
            {children}
        </main>
    );
}

export default withStyles(styles)(AppContent);