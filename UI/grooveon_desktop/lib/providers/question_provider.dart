import 'package:grooveon_desktop/models/response/question_response.dart';
import 'package:grooveon_desktop/providers/base_provider.dart';

class QuestionProvider extends BaseProvider<QuestionResponse> {
  QuestionProvider() : super("Question");

  @override
  QuestionResponse fromJson(data) {
    return QuestionResponse.fromJson(data);
  }
}