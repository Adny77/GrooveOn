import 'package:grooveon_desktop/models/response/answer_response.dart';
import 'package:grooveon_desktop/providers/base_provider.dart';

class AnswerProvider extends BaseProvider<AnswerResponse> {
  AnswerProvider() : super("Answer");

  @override
  AnswerResponse fromJson(data) {
    return AnswerResponse.fromJson(data);
  }
}