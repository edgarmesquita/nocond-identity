import React from "react";
import {
    withStyles,
    createStyles,
    Theme,
    WithStyles,
    StyleRules
} from "@material-ui/core/styles";
import { CenteredPaper } from "../components";


import "../assets/css/stretch.css";

const styles: (theme: Theme) => StyleRules<string> = theme =>
    createStyles({
        root: {

        },

    });

type ResetPasswordPageProps = {

}
    & WithStyles<typeof styles>;

const ResetPasswordPage = ({ classes }: ResetPasswordPageProps) => {
    return (
        <CenteredPaper>

        </CenteredPaper>
    )
}

export default withStyles(styles)(ResetPasswordPage);