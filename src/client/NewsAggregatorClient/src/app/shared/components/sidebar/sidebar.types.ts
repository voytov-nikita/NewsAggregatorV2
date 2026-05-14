export interface NavChild {
  id: string;
  label: string;
  route: string;
}

export interface NavItem {
  id: string;
  label: string;
  icon: string;
  route?: string;
  comingSoon?: boolean;
  children?: NavChild[];
}
