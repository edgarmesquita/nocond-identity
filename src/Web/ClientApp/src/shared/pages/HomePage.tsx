import React, { ReactNode } from "react";
import { isMobile } from 'react-device-detect';
import { withCookies, ReactCookieProps } from "react-cookie";
import {
    withStyles,
    createStyles,
    Theme,
    WithStyles,
    StyleRules
} from "@material-ui/core/styles";
import { RouteComponentProps } from "react-router-dom";
import AppLayout from "../components/AppLayout";
import { Button, Card, CardActions, CardContent, Container, Grid, Typography } from "@material-ui/core";

const styles: (theme: Theme) => StyleRules<string> = theme =>
    createStyles({
        root: {
            display: 'flex',
        },
        avatar: {
            width: theme.spacing(12),
            height: theme.spacing(12),
            backgroundColor: '#FFF',
            marginBottom: theme.spacing(2)
        },
        drawerProfile: {
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            flexDirection: 'column',
        }
    });

type HomeProps = { children?: ReactNode } & ReactCookieProps & WithStyles<typeof styles> & RouteComponentProps;

const HomePage = ({ classes, children, cookies, history, ...props }: HomeProps) => {

    const handleClick = (event: React.MouseEvent<HTMLButtonElement, Event>) => {
        history.push('/personal-info')
    }
    return (
        <AppLayout history={history} {...props}>
            <Container>
                <Typography align="center" variant="h4" component="h3">Bem-vindo</Typography>
                <Typography align="center" variant="h6" component="h4" gutterBottom>Gerencie suas informações</Typography>

                <Grid container spacing={2}>
                    <Grid item xs={12} sm={6}>
                        <Card>
                            <CardContent>
                                <Typography gutterBottom variant="h5" component="h5">
                                    Informações Pessoais
                                </Typography>
                                <Typography variant="body2" color="textSecondary" component="p">
                                    Informações básicas, como seu nome e foto, usadas na plataforma NoCond
                                </Typography>
                            </CardContent>
                            <CardActions>
                                <Button size="small" color="primary" onClick={handleClick}>
                                    Gerenciar minhas informações
                                </Button>
                            </CardActions>
                        </Card>
                    </Grid>
                </Grid>
            </Container>
        </AppLayout>
    );
}

export default withCookies(withStyles(styles)(HomePage));