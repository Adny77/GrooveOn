import 'package:grooveon_desktop/models/response/role_response.dart';
import 'base_provider.dart';

class RoleProvider extends BaseProvider<RoleResponse> {
  RoleProvider() : super("Role");

  @override
  RoleResponse fromJson(data) {
    return RoleResponse.fromJson(data);
  }
}
