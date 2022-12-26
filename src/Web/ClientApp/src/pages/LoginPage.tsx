import React, { useCallback } from "react";
import { useDispatch, useSelector } from 'react-redux';
import { TextValidator, ValidatorForm } from 'react-material-ui-form-validator';
import {
    Grid, Button, FormControlLabel, Checkbox,
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
import queryString from 'query-string';

import { Authenticator, CenteredPaper } from "../components";
import { getModelState, getRedirectPath, getStatus, loginUserRequest, reset, setSignInStatus } from "../store/auth/actions";
import { SignInStatus } from "../store/auth/types";

import "../assets/css/stretch.css";
import logo from "../assets/images/logo.png";
import { useHistory, useLocation } from "react-router-dom";

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

type LoginProps = {

} & WithStyles<typeof styles>;

const LoginPage = ({
    classes
}: LoginProps) => {
    const dispatch = useDispatch();
    const redirectPath = useSelector(getRedirectPath);
    const modelState = useSelector(getModelState);
    const status = useSelector(getStatus);
    const [email, setEmail] = React.useState<string>("");
    const [password, setPassword] = React.useState<string>("");
    const [rememberMe, setRememberMe] = React.useState<boolean>(false);
    const [showPassword, setShowPassword] = React.useState<boolean>(false);
    const history = useHistory();

    const onFailedAuth = useCallback((): void => {
        dispatch(reset());
        dispatch(setSignInStatus(SignInStatus.None));
    }, [dispatch]);

    const onSuccessfulAuth = useCallback((): void => { window.location.href = redirectPath; }, [redirectPath]);

    const handleEmailChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setEmail(event.target.value);
    };

    const handlePasswordChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setPassword(event.target.value);
    };

    const handleClickShowPassword = () => {
        setShowPassword(!showPassword);
    };

    const handleMouseDownPassword = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
    };

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {

        //if (status === SignInStatus.Process) return;

        dispatch(setSignInStatus(SignInStatus.Process));

        dispatch(loginUserRequest({
            email: email,
            password: password,
            rememberMe: rememberMe
        }, encodeURIComponent(params.ReturnUrl)));
    }

    const params = queryString.parse(history.location.search)

    return (
        <CenteredPaper>

            <ValidatorForm onSubmit={handleSubmit}>
                <Typography variant="h4" component="h1" gutterBottom>
                    <img src={logo} alt="NoCond" className={classes.logo} />
                </Typography>

                <Authenticator
                    authStatus={status} modelState={modelState}
                    handleOnFail={onFailedAuth}
                    handleOnSuccess={onSuccessfulAuth}
                />
                <Grid container spacing={8} alignItems="flex-end" style={{ marginTop: '10px' }}>

                    <Grid item md={true} sm={true} xs={true}>
                        <TextValidator id="email" name="email" label="E-mail" type="email"
                            value={email} className={classes.textField}
                            variant="outlined" onChange={handleEmailChange}
                            fullWidth autoFocus required
                            validators={['required']}
                            errorMessages={['O e-mail é obrigatório']}
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
                        <TextValidator id="password" name="password" label="Palavra-chave" className={classes.textField}
                            type={showPassword ? 'text' : 'password'}
                            value={password} variant="outlined"
                            onChange={handlePasswordChange}
                            fullWidth required
                            validators={['required']}
                            errorMessages={['A senha é obrigatória']}
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
                <Grid container alignItems="center" justify="space-between">
                    <Grid item>
                        <FormControlLabel control={
                            <Checkbox id="rememberMe" name="rememberMe"
                                color="primary"
                            />
                        } label="Remember me" />
                    </Grid>
                    <Grid item>
                        <Button disableFocusRipple disableRipple style={{ textTransform: "none" }} variant="text" color="primary">Não te lembras da palavra-chave?</Button>
                    </Grid>
                </Grid>
                <Grid container justify="center" style={{ marginTop: '30px' }}>
                    <Button type="submit" variant="contained" size="large" color="primary" fullWidth>Entrar</Button>
                </Grid>
            </ValidatorForm>

        </CenteredPaper>
    );
}

export default withStyles(styles)(LoginPage);