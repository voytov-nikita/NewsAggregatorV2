import { createFeature, createReducer, on } from '@ngrx/store';
import { UiActions } from './ui.actions';

export type Theme = 'dark' | 'light';

export interface UiState {
  theme: Theme;
  sidebarCollapsed: boolean;
}

const THEME_STORAGE_KEY = 'na-theme';
const SIDEBAR_STORAGE_KEY = 'na-sidebar-collapsed';

const readTheme = (): Theme => {
  try {
    return localStorage.getItem(THEME_STORAGE_KEY) === 'light' ? 'light' : 'dark';
  } catch {
    return 'dark';
  }
};

const readCollapsed = (): boolean => {
  try {
    return localStorage.getItem(SIDEBAR_STORAGE_KEY) === '1';
  } catch {
    return false;
  }
};

export const initialUiState: UiState = {
  theme: readTheme(),
  sidebarCollapsed: readCollapsed(),
};

export const uiFeature = createFeature({
  name: 'ui',
  reducer: createReducer(
    initialUiState,
    on(UiActions.setTheme, (state, { theme }) => ({ ...state, theme })),
    on(UiActions.toggleTheme, (state) => ({
      ...state,
      theme: state.theme === 'dark' ? 'light' : 'dark',
    })),
    on(UiActions.toggleSidebar, (state) => ({
      ...state,
      sidebarCollapsed: !state.sidebarCollapsed,
    })),
    on(UiActions.setSidebarCollapsed, (state, { collapsed }) => ({
      ...state,
      sidebarCollapsed: collapsed,
    })),
  ),
});

export const {
  name: uiFeatureKey,
  reducer: uiReducer,
  selectTheme,
  selectSidebarCollapsed,
} = uiFeature;
