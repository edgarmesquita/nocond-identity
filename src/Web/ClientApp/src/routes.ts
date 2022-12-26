import { HomePage, LoginPage } from "./pages";
import PersonalInfoPage from "./user/pages/PersonalInfoPage";

export interface ITreeItem {
    title: string;
    icon?: string;
    path?: string | string[];
    exact?: boolean;
    visible?: boolean;
    protected?: boolean;
    component?: any;
    children?: ITreeItem[];
    params?: { [paramName: string]: string }[]
}
const routes: ITreeItem[] = [
    { title: "Login", path: '/login', exact: true, component: LoginPage },

    { title: "Início", icon: "home", path: '/', exact: true, visible: true, component: HomePage },
    { title: "Info. Pessoais", icon: "assignment_ind", path: '/personal-info', exact: true, visible: true, component: PersonalInfoPage },
];

export default routes;