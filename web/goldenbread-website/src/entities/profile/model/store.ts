import { defineStore } from 'pinia';
import { getProfile } from '../api/getProfile';
import { updateRequisites } from '../api/updateRequisites';
import { updateContacts } from '../api/updateContacts';
import { changePassword } from '../api/changePassword';
import { changeEmail } from '../api/changeEmail';
import type { 
  ProfileResponse, 
  UpdateRequisitesRequest, 
  UpdateContactsRequest,
  ChangePasswordRequest,
  ChangeEmailRequest 
} from './types';
import { useUserStore } from '@/entities/user';

const useUser = useUserStore();

export const useProfileStore = defineStore('profile', {
  state: () => ({
    profile: null as ProfileResponse | null,
  }),

  getters: {
    isProfileLoaded: (state) => state.profile !== null,
    
    requisites: (state) => state.profile ? {
      name: state.profile.name,
      inn: state.profile.inn,
      ogrn: state.profile.ogrn,
    } : null,
    
    contacts: (state) => state.profile ? {
      phone: state.profile.phone,
      address: state.profile.address,
    } : null,
    
    credentials: (state) => state.profile ? {
      email: state.profile.email,
    } : null,
  },

  actions: {
    setProfile(profile: ProfileResponse) {
      this.profile = profile;
    },

    async loadProfile() {
      const profile = await getProfile();
      this.setProfile(profile);
      return profile;
    },

    async updateRequisites(payload: UpdateRequisitesRequest) {
      await updateRequisites(payload);
      useUser.logout();
    },

    async updateContacts(payload: UpdateContactsRequest) {
      await updateContacts(payload);
      if (this.profile) {
        this.profile.phone = payload.phone;
        this.profile.address = payload.address;
      }
    },

    async changePassword(payload: ChangePasswordRequest) {
      await changePassword(payload);
    },

    async changeEmail(payload: ChangeEmailRequest) {
      await changeEmail(payload);
      useUser.logout();
    },
  },
});