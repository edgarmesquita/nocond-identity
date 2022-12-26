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
import { Avatar, Box, Typography } from "@material-ui/core";
import { RouteComponentProps, generatePath } from "react-router-dom";
import { MenuBar, MenuDrawer, UserCookieConsent } from "@nocond/components";
import AppContent from "./AppContent";

import "../../assets/css/stretch.css";
import user from '../../assets/images/laptop-user.png';
import routes, { ITreeItem } from "../../routes";
import { IMenuDrawerItem } from "@nocond/components/dist/menus/MenuDrawer";

const styles: (theme: Theme) => StyleRules<string> = theme =>
    createStyles({
        root: {
            display: 'flex',
        },
        avatar: {
            width: theme.spacing(12),
            height: theme.spacing(12),
            backgroundColor: '#FFF',
            marginBottom: `-${theme.spacing(8)}px`,
            borderWidth: theme.spacing(1),
            borderColor: '#333',
            borderStyle: 'solid'
        },
        userName: {
            marginBottom: theme.spacing(2)
        },
        drawerProfile: {
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            flexDirection: 'column',
            backgroundColor: '#444',
            //backgroundColor: theme.palette.primary.main,
            marginBottom: theme.spacing(5)
        },
        input: {
            color: '#FFF !important'
        }
    });

export interface IHomeState {
    currentRoom: number;
    currentSubRoom?: number;

}

type AppLayoutProps = {
    children?: ReactNode
} & ReactCookieProps & WithStyles<typeof styles> & RouteComponentProps;

const AppLayout = ({ classes, history, match, children, cookies }: AppLayoutProps) => {
    const drawerOpen = cookies?.get<string>('drawer') === 'opened';
    const currentItem = cookies?.get<string>('drawer-currentitem');

    const handleDrawerItemClick = (path: string) => () => {
        history.push(path);
        cookies?.set('drawer-currentitem', path);

        if (isMobile) {
            cookies?.set('drawer', 'closed');
        }
    }

    const getMenu = (currentPath: string | undefined = undefined, items: ITreeItem[] | null = null, main: IMenuDrawerItem | null = null, level: number = 0): any[] => {
        const list = items || routes;
        return list.filter(item => item.visible).map(item => {
            let path = item.path ? (Array.isArray(item.path) ? item.path[0] : item.path) : undefined;
            if (!path && main?.path && item.params) {
                const mainPath = Array.isArray(main.path) ? main.path[0] : main.path;
                path = generatePath(mainPath, item.params[0]);
            }
            let menuItem: IMenuDrawerItem = {
                title: item.title,
                icon: item.icon,
                path: path,
                onClick: path && !item.children ? handleDrawerItemClick(path) : undefined
            };
            menuItem.open = currentPath !== undefined && currentPath === path;
            menuItem.children = item.children ? getMenu(currentPath, item.children, menuItem, level + 1) : undefined;

            if (menuItem.children && !menuItem.open) {
                menuItem.open = menuItem.children.some(o => o.open);
            }
            return menuItem;
        })
    }

    const handleDrawerOpen = () => {
        cookies?.set('drawer', 'opened');
    };

    const handleDrawerClose = () => {
        cookies?.set('drawer', 'closed');
    };

    return (
        <div className={classes.root}>
            <MenuBar title="NoCond" onDrawerOpen={handleDrawerOpen} open={drawerOpen} />
            <MenuDrawer
                onDrawerClose={handleDrawerClose}
                onDrawerOpen={handleDrawerOpen}
                open={drawerOpen} items={getMenu(currentItem)}>
                {drawerOpen && (<Box p={2} className={classes.drawerProfile}>
                    <Typography variant="subtitle2" className={classes.userName}>Administrador</Typography>
                    <Avatar src={user} className={classes.avatar} />

                </Box>)}

            </MenuDrawer>
            <AppContent drawerOpen={!isMobile && drawerOpen}>
                {children}
            </AppContent>
            <UserCookieConsent />
        </div>
    );
}

export default withCookies(withStyles(styles)(AppLayout));