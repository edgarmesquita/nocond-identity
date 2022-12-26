import React from "react";
import { History } from 'history';

import {
    Grid, TextField, Button,
    InputAdornment, IconButton, Typography
} from '@material-ui/core';
import {
    Face, Fingerprint, Visibility, VisibilityOff
} from '@material-ui/icons'
import {
    withStyles,
    createStyles,
    Theme,
    WithStyles,
    StyleRules
} from "@material-ui/core/styles";

import { AuthStatusEnum } from '../store/auth/types';

import "../assets/css/stretch.css";
import logo from "../assets/images/logo.png";
import CenteredPaper from "../components/CenteredPaper";
import {useDispatch} from "react-redux";

const styles: (theme: Theme) => StyleRules<string> = theme =>
    createStyles({
        root: {

        },
        textField: {
            "& input:-webkit-autofill, input:-webkit-autofill:hover, input:-webkit-autofill:focus, textarea:-webkit-autofill, textarea:-webkit-autofill:hover, textarea:-webkit-autofill:focus, select:-webkit-autofill, select:-webkit-autofill:hover, select:-webkit-autofill:focus": {
                backgroundColor: "transparent !important"
            }
        },
        logo: {
            maxWidth: "100%"
        }
    });

type RegisterProps = { history: History }
    & WithStyles<typeof styles>;

const RegisterPage = ({ classes }: RegisterProps) => {
    const dispatch = useDispatch();
    const [email, setEmail] = React.useState<string>("");
    const [password, setPassword] = React.useState<string>("");
    const [showPassword, setShowPassword] = React.useState<boolean>(false);
    const [confirmPassword, setConfirmPassword] = React.useState<string>("");

    const handleEmailChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setEmail(event.target.value);
    };

    const handlePasswordChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setPassword(event.target.value);
    };

    const handleConfirmPasswordChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setConfirmPassword(event.target.value);
    };

    const handleMouseDownPassword = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
    };

    const handleClickShowPassword = () => {
        setShowPassword(!showPassword);
    };

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        if (status === AuthStatusEnum.PROCESS) return; // TODO: Verificar 

        // registerUserRequest({
        //     email: email,
        //     password: password,
        //     confirmPassword: confirmPassword
        // });

    }
    return (
        <CenteredPaper>
            <form onSubmit={handleSubmit}>
                <Typography variant="h4" component="h1" gutterBottom>
                    <img src={logo} alt="NoCond" className={classes.logo} />
                </Typography>
                <Grid container spacing={8} alignItems="flex-end" style={{ marginTop: '10px' }}>

                    <Grid item md={true} sm={true} xs={true}>
                        <TextField id="email" name="email" label="E-mail" type="email"
                            value={email} className={classes.textField}
                            variant="outlined" onChange={handleEmailChange}
                            fullWidth autoFocus required
                            InputProps={{
                                startAdornment: (
                                    <InputAdornment position="start">
                                        <Face />
                                    </InputAdornment>
                                ),
                            }} />
                    </Grid>
                </Grid>
                <Grid container spacing={8} alignItems="flex-end">
                    <Grid item md={true} sm={true} xs={true}>
                        <TextField id="password" name="password" label="Palavra-chave" className={classes.textField}
                            type={showPassword ? 'text' : 'password'}
                            value={password} variant="outlined"
                            onChange={handlePasswordChange}
                            fullWidth required
                            InputProps={{
                                startAdornment: (
                                    <InputAdornment position="start">
                                        <Fingerprint />
                                    </InputAdornment>
                                ),
                                endAdornment: (
                                    <InputAdornment position="end">
                                        <IconButton
                                            aria-label="toggle password visibility"
                                            onClick={handleClickShowPassword}
                                            onMouseDown={handleMouseDownPassword}
                                            edge="end">
                                            {showPassword ? <Visibility /> : <VisibilityOff />}
                                        </IconButton>
                                    </InputAdornment>
                                )
                            }} />

                    </Grid>
                </Grid>
                <Grid container spacing={8} alignItems="flex-end">
                    <Grid item md={true} sm={true} xs={true}>
                        <TextField id="confirmPassword" name="confirmPassword" label="Confirmar Palavra-chave" className={classes.textField}
                            type='password'
                            value={confirmPassword} variant="outlined"
                            onChange={handleConfirmPasswordChange}
                            fullWidth required
                            InputProps={{
                                startAdornment: (
                                    <InputAdornment position="start">
                                        <Fingerprint />
                                    </InputAdornment>
                                ),
                                endAdornment: (
                                    <InputAdornment position="end">

                                    </InputAdornment>
                                )
                            }} />

                    </Grid>
                </Grid>
                <Grid container justify="center" style={{ marginTop: '30px' }}>
                    <Button type="submit" variant="contained" size="large" color="primary" fullWidth>Registrar</Button>
                </Grid>
            </form>
        </CenteredPaper>
    );
}

export default withStyles(styles)(RegisterPage);