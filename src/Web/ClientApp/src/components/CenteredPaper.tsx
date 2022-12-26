import React, { ReactNode } from "react";
import {
    withStyles,
    createStyles,
    Theme,
    WithStyles,
    StyleRules
} from "@material-ui/core/styles";

import { Paper } from "@material-ui/core";

import "../assets/css/stretch.css";

const styles: (theme: Theme) => StyleRules<string> = theme =>
    createStyles({
        root: {
            display: 'flex',
            height: "100%",
            alignItems: 'stretch',

        },
        content: {
            flexGrow: 1,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            padding: theme.spacing(3),
            backgroundImage: "linear-gradient(to right bottom, #0a3050, #1c4061, #2c5072, #3c6183, #4c7395)",
            [theme.breakpoints.down('sm')]: {
                padding: theme.spacing(1),
            },
        },
        paperInner: {
            margin: theme.spacing(4),
        },
        paper: {
            padding: theme.spacing(1),
            maxWidth: "500px"
        },
    });

type StudentRegisterPageProps = {
    children: ReactNode;
} & WithStyles<typeof styles>;

const StudentRegisterPage = ({ classes, children }: StudentRegisterPageProps) => {


    return (
        <div className={classes.root}>
            <main className={classes.content}>
                <Paper className={classes.paper}>
                    <div className={classes.paperInner}>
                        {children}
                    </div>
                </Paper>
            </main>
        </div>
    );
}

export default withStyles(styles)(StudentRegisterPage);