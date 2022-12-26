import React, { ReactNode } from "react";
import clsx from 'clsx';
import {
    withStyles,
    createStyles,
    Theme,
    WithStyles,
    StyleRules
} from "@material-ui/core/styles";
import { RouteComponentProps } from "react-router-dom";
import AppLayout from "../../shared/components/AppLayout";
import { Avatar, Button, Card, CardActions, CardContent, Container, Divider, List, ListItem, ListItemAvatar, ListItemIcon, ListItemSecondaryAction, ListItemText, Typography } from "@material-ui/core";
import ArrowForwardIosIcon from '@material-ui/icons/ArrowForwardIos';
import TextFieldsIcon from '@material-ui/icons/TextFields';
const styles: (theme: Theme) => StyleRules<string> = theme =>
    createStyles({
        list: {
            width: '100%',
            backgroundColor: theme.palette.background.paper,
        },
    });

type PersonalInfoPageProps = {

} & WithStyles<typeof styles> & RouteComponentProps;

const PersonalInfoPage = ({ classes, ...props }: PersonalInfoPageProps) => {

    const handleClick = (event: React.MouseEvent<HTMLButtonElement, Event>) => {

    }
    return (
        <AppLayout {...props}>
            <Container>
                <Typography align="center" variant="h4" component="h3">Informações Pessoais</Typography>
                <Typography align="center" variant="h6" component="h4" gutterBottom>
                    Informações básicas, como seu nome e foto, usadas na plataforma NoCond
                </Typography>

                <Card>
                    <CardContent>
                        <Typography gutterBottom variant="h5" component="h5">
                            Informações Básicas
                        </Typography>
                        <Typography variant="body2" color="textSecondary" component="p">
                            Informações básicas, como seu nome e foto, usadas na plataforma NoCond
                        </Typography>

                        <List className={classes.list}>
                            <ListItem button>
                                <ListItemAvatar>
                                    <Avatar alt="Remy Sharp" src="/static/images/avatar/1.jpg" />
                                </ListItemAvatar>
                                <ListItemText primary="Foto" secondary="Uma foto ajuda a personalizar sua conta" />

                                <ListItemSecondaryAction>
                                    <ArrowForwardIosIcon fontSize="small" />
                                </ListItemSecondaryAction>
                            </ListItem>
                            <Divider />
                            <ListItem button>
                                <ListItemIcon>
                                    <TextFieldsIcon />
                                </ListItemIcon>
                                <ListItemText primary="Nome" secondary="Administrador" />

                                <ListItemSecondaryAction>
                                    <ArrowForwardIosIcon fontSize="small" />
                                </ListItemSecondaryAction>
                            </ListItem>
                            <Divider />
                            <ListItem button>
                                <ListItemIcon>
                                    <TextFieldsIcon />
                                </ListItemIcon>
                                <ListItemText primary="Data de Nascimento" secondary="01/01/2001" />

                                <ListItemSecondaryAction>
                                    <ArrowForwardIosIcon fontSize="small" />
                                </ListItemSecondaryAction>
                            </ListItem>
                            <Divider />
                            <ListItem button>
                                <ListItemIcon>
                                    <TextFieldsIcon />
                                </ListItemIcon>
                                <ListItemText primary="Gênero" secondary="Masculino" />

                                <ListItemSecondaryAction>
                                    <ArrowForwardIosIcon fontSize="small" />
                                </ListItemSecondaryAction>
                            </ListItem>
                            <Divider />
                            <ListItem button>
                                <ListItemIcon>
                                    <TextFieldsIcon />
                                </ListItemIcon>
                                <ListItemText primary="Senha" secondary="••••••••" />

                                <ListItemSecondaryAction>
                                    <ArrowForwardIosIcon fontSize="small" />
                                </ListItemSecondaryAction>
                            </ListItem>
                        </List>
                    </CardContent>
                </Card>

                <Card>
                    <CardContent>
                        <Typography gutterBottom variant="h5" component="h5">
                            Informações de Contato
                        </Typography>

                        <List className={classes.list}>
                            <ListItem button>
                                <ListItemAvatar>
                                    <Avatar alt="Remy Sharp" src="/static/images/avatar/1.jpg" />
                                </ListItemAvatar>
                                <ListItemText primary="Foto" secondary="Uma foto ajuda a personalizar sua conta" />

                                <ListItemSecondaryAction>
                                    <ArrowForwardIosIcon fontSize="small" />
                                </ListItemSecondaryAction>
                            </ListItem>
                            <Divider />
                            <ListItem button>
                                <ListItemIcon>
                                    <TextFieldsIcon />
                                </ListItemIcon>
                                <ListItemText primary="Nome" secondary="Administrador" />

                                <ListItemSecondaryAction>
                                    <ArrowForwardIosIcon fontSize="small" />
                                </ListItemSecondaryAction>
                            </ListItem>
                            <Divider />
                            <ListItem button>
                                <ListItemIcon>
                                    <TextFieldsIcon />
                                </ListItemIcon>
                                <ListItemText primary="Data de Nascimento" secondary="01/01/2001" />

                                <ListItemSecondaryAction>
                                    <ArrowForwardIosIcon fontSize="small" />
                                </ListItemSecondaryAction>
                            </ListItem>
                            <Divider />
                            <ListItem button>
                                <ListItemIcon>
                                    <TextFieldsIcon />
                                </ListItemIcon>
                                <ListItemText primary="Gênero" secondary="Masculino" />

                                <ListItemSecondaryAction>
                                    <ArrowForwardIosIcon fontSize="small" />
                                </ListItemSecondaryAction>
                            </ListItem>
                            <Divider />
                            <ListItem button>
                                <ListItemIcon>
                                    <TextFieldsIcon />
                                </ListItemIcon>
                                <ListItemText primary="Senha" secondary="••••••••" />

                                <ListItemSecondaryAction>
                                    <ArrowForwardIosIcon fontSize="small" />
                                </ListItemSecondaryAction>
                            </ListItem>
                        </List>
                    </CardContent>
                </Card>
            </Container>
        </AppLayout>
    );
}

export default withStyles(styles)(PersonalInfoPage);